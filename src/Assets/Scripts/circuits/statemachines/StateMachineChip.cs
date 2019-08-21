namespace AssemblyCSharp
{
	using System.Collections.Generic;

	public class StateMachineChip : Chip
	{
		private int timeInState = 0;

		private StateChip activeState;
		private StateChip prevActiveState;

		public StateMachineChip(CircuitManager manager) : base(manager, 1, 5, true, StatePort.StatePortType.Root)
		{
			EmitEvaluationRequired();
		}

		override public int IconIndex
		{
			get
			{
				return 31;
			}
		}

		private void ResetActiveState()
		{
			activeState = InitialState();
			if (activeState != null)
				activeState.Active = true;
			timeInState = 0;
		}

		public StateChip InitialState()
		{
			DebugUtils.Assert(statePort.connections.Count <= 1);
			if (statePort.connections.Count > 0)
				return (StateChip)statePort.connections[0].targetPort.node;
			return null;
		}

		private bool isActive()
		{
			return !IsResetSet && !inputPorts[0].IsConnected || inputPorts[0].GetValue() != 0;
		}

		public override void Evaluate()
		{
			DebugUtils.Assert(statePort != null);

			if (IsResetSet)
			{
				for (int i = 0; i < outputPortCount; ++i)
					outputPorts[i].Value = 0;
			}
			else if (isActive())
			{
				prevActiveState = activeState;
				activeState = FindActiveState();

				timeInState++;

				if (activeState == null)
					ResetActiveState();

				if (activeState != null && timeInState > 0 && timeInState >= activeState.MinTimeInState)
				{
					StateChip nextState = NextStateAfterValidTransition();
					if (nextState != null)
					{
						activeState.Active = false;
						nextState.Active = true;
						activeState = nextState;
						timeInState = 0;
					}
				}

				EvaluateOutputs();

				EmitEvaluationRequired();
			}

			outputPorts[outputPortCount].Value = ResetValue;
		}

		override protected void EvaluateOutputs()
		{
			if (activeState != null)
			{
				outputPorts[0].Value = activeState.Value0;
				outputPorts[1].Value = activeState.Value1;
				outputPorts[2].Value = activeState.Value2;
				outputPorts[3].Value = timeInState * 100;
			}
			else
			{
				outputPorts[0].Value = 0;
				outputPorts[1].Value = 0;
				outputPorts[2].Value = 0;
				outputPorts[3].Value = 0;
			}

			outputPorts[4].Value = prevActiveState != activeState ? 1 : 0;
		}

		private StateChip FindActiveState()
		{
			if (statePort.connections.Count == 0)
				return null;
			HashSet<StatePort> visited = new HashSet<StatePort>();
			visited.Add(statePort);
			return FindActiveState((StatePort)statePort.connections[0].targetPort, visited);
		}

		private StateChip FindActiveState(StatePort port, HashSet<StatePort> visited)
		{
			if (visited.Contains(port))
				return null;
			DebugUtils.Assert(!port.isRootPort);
			visited.Add(port);

			if (((StateChip)port.node).Active)
				return (StateChip)port.node;

			foreach (Connection connection in port.connections)
			{
				var otherPort = connection.getOtherPort(port);
				if (otherPort != null && otherPort.IsState)
				{
					StateChip active = FindActiveState((StatePort)otherPort, visited);
					if (active != null)
						return active;
				}
			}
			return null;
		}

		public StateChip NextStateAfterValidTransition()
		{
			StatePort port = activeState.statePort;
			//TODO: Sort port.connections or make connections generally sorted.
			foreach (StateMachineTransition connection in port.connections)
			{
				if (object.ReferenceEquals(connection.sourcePort, port))
				{
					DebugUtils.Assert(connection.targetPort != null);
					DebugUtils.Assert(connection.targetPort.IsState);
					if (ToBool(connection.transitionEnabledPort))
						return (StateChip)connection.targetPort.node;
				}
			}
			return null;
		}

		public override IEnumerable<CircuitNode> SimpleDependsOn()
		{
			foreach (var port in FindTransitionEnabledPorts())
			{
				if (port.IsConnected && !ReferenceEquals(port.connections[0].sourcePort.node, this))
					yield return port.connections[0].sourcePort.node;
			}
		}

		public override IEnumerable<CircuitNode> SimpleDependingOnThis()
		{
			//TODO
			throw new System.NotImplementedException();
		}

		public IEnumerable<InputPort> FindTransitionEnabledPorts()
		{
			return FindTransitionEnabledPorts(statePort, new HashSet<StatePort>());
		}

		private IEnumerable<InputPort> FindTransitionEnabledPorts(StatePort port, HashSet<StatePort> visited)
		{
			visited.Add(port);
			if (visited.Contains(port))
				yield break;

			foreach (StateMachineTransition transition in port.connections)
			{
				if (object.ReferenceEquals(transition.sourcePort, port))
				{
					if (transition.transitionEnabledPort != null)
						yield return transition.transitionEnabledPort;
				}
				else
				{
					DebugUtils.Assert(transition.targetPort != null);
					DebugUtils.Assert(transition.targetPort.IsState);
					foreach (InputPort result in FindTransitionEnabledPorts((StatePort)transition.targetPort, visited))
						yield return result;
				}
			}
		}
	}
}
