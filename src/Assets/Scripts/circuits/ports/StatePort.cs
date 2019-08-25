namespace AssemblyCSharp
{
	using System.Collections.Generic;

	public class StatePort : Port
	{
		public StatePort(CircuitNode node, bool isRoot) : base(node, isRoot ? PortType.StateRoot : PortType.StatePort)
		{
			Connected += StatePort_Connected;
			Disconnected += StatePort_Disconnected;
		}

		public override bool Connect(Port port)
		{
			if (!port.IsStatePort)
				return false;
			if (object.ReferenceEquals(port, this))
				return false;
			if (port.IsStateRootPort)
				return port.Connect(this);

			StatePort source = this;
			StatePort target = (StatePort)port;

			Connection existingTransition = connections.Find(t => object.ReferenceEquals(t.SourcePort, source) && object.ReferenceEquals(t.TargetPort, target));
			if (existingTransition != null)
			{
				existingTransition.Disconnect();
				return true;
			}

			StateMachineChip targetRoot;
			StateMachineChip sourceRoot;
			{
				if (source.IsStateRootPort)
					sourceRoot = (StateMachineChip)source.node;
				else
					sourceRoot = ((StateChip)source.node).StateMachine;
				targetRoot = ((StateChip)target.node).StateMachine;
			}

			if (sourceRoot != null && targetRoot != null && !object.ReferenceEquals(sourceRoot, targetRoot))
				return false;

			if (source.IsStateRootPort)
				source.DisconnectAll();

			StateMachineTransition transition = new StateMachineTransition(source, target);
			connections.Add(transition);
			port.connections.Add(transition);
			EmitConnected(transition);
			return true;
		}

		public IEnumerable<StateMachineTransition> GetOutgoingTransitions()
		{
			foreach (StateMachineTransition connection in connections)
				if (object.ReferenceEquals(connection.SourcePort, this))
					yield return connection;
		}

		private void StatePort_Connected(Connection connection)
		{
			DebugUtils.Assert(object.ReferenceEquals(connection.SourcePort, this));
			DebugUtils.Assert(!connection.TargetPort.IsStateRootPort);
			StateMachineTransition transition = (StateMachineTransition)connection;
			StateMachineChip stateMachine;
			StateChip targetState = (StateChip)transition.TargetStatePort.node;
			if (transition.SourceStatePort.IsStateRootPort)
				stateMachine = (StateMachineChip)transition.SourceStatePort.node;
			else
			{
				StateChip sourceState = (StateChip)transition.SourceStatePort.node;
				stateMachine = sourceState.StateMachineUnchecked;
				if (stateMachine == null)
					stateMachine = targetState.StateMachineUnchecked;
			}
			if (stateMachine == null)
			{
				if (!transition.SourceStatePort.IsStateRootPort)
					((StateChip)transition.SourceStatePort.node).CheckStateMachine();
				targetState.CheckStateMachine();
				return;
			}

			StateChip newState;
			if (targetState.StateMachineUnchecked == null)
				newState = targetState;
			else
				newState = (StateChip)transition.SourceStatePort.node;

			if (newState.StateMachineUnchecked == null)
			{
				DebugUtils.Assert(newState.statePort.connections.Count > 0);
				if (newState.statePort.connections.Count > 1)
					stateMachine.UpdateConnectedStates();
				else
				{
					DebugUtils.Assert(!stateMachine.connectedStates.Contains(newState));
					stateMachine.connectedStates.Add(newState);
					newState.StateMachine = stateMachine;
				}
			}
			else
				DebugUtils.Assert(object.ReferenceEquals(targetState.StateMachine, stateMachine));
			if (!transition.SourceStatePort.IsStateRootPort)
				((StateChip)transition.SourceStatePort.node).CheckStateMachine();
			targetState.CheckStateMachine();
		}

		private void StatePort_Disconnected(Connection connection)
		{
			if (!object.ReferenceEquals(connection.SourcePort, this))
				return;
			StateMachineTransition transition = (StateMachineTransition)connection;
			StateMachineChip stateMachine;
			StateChip targetState = (StateChip)transition.TargetStatePort.node;
			targetState.Active = false;
			if (transition.SourceStatePort.IsStateRootPort)
				stateMachine = (StateMachineChip)transition.SourceStatePort.node;
			else
			{
				StateChip sourceState = (StateChip)transition.SourceStatePort.node;
				stateMachine = sourceState.StateMachineUnchecked;
			}
			DebugUtils.Assert(object.ReferenceEquals(stateMachine, targetState.StateMachineUnchecked));
			if (stateMachine == null)
			{
				if (!transition.SourceStatePort.IsStateRootPort)
					((StateChip)transition.SourceStatePort.node).CheckStateMachine();
				targetState.CheckStateMachine();
				return;
			}
			if (transition.TargetStatePort.connections.Count > 0)
				stateMachine.UpdateConnectedStates();
			else
			{
				stateMachine.connectedStates.Remove(targetState);
				targetState.StateMachine = null;
			}
			if (!transition.SourceStatePort.IsStateRootPort)
				((StateChip)transition.SourceStatePort.node).CheckStateMachine();
			targetState.CheckStateMachine();
		}
	}
}
