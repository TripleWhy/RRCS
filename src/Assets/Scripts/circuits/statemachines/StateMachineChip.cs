namespace AssemblyCSharp
{
	using System.Linq;
	using System.Collections.Generic;

	public class StateMachineChip : Chip
	{
		private int timeInState = 0;
		private StateChip activeState;
		public List<StateChip> connectedStates = new List<StateChip>();

		public StateMachineChip(CircuitManager manager)
			: base(manager, 1, 5, true, Port.PortType.StateRoot)
		{
			inputPorts[0].UnconnectedValue = 1;
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
				return (StateChip)statePort.connections[0].TargetPort.node;
			return null;
		}

		private bool IsActive
		{
			get
			{
				return InBool(0);
			}
		}

		public override void Evaluate()
		{
			DebugUtils.Assert(statePort != null);

			if (IsResetSet)
			{
				if (activeState != null)
					activeState.Active = false;
				ResetActiveState();
				for (int i = 0; i < outputPortCount; ++i)
					outputPorts[i].Value = 0;
			}
			else if (IsActive)
			{
				StateChip lastActiveState = activeState;
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
				outputPorts[4].Value = object.ReferenceEquals(lastActiveState, activeState) ? 0 : 1;

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
		}

		private StateChip FindActiveState()
		{
			foreach (StateChip state in connectedStates)
				if (state.Active)
					return state;
			return null;
		}

		public void UpdateConnectedStates()
		{
			foreach (StateChip state in connectedStates)
				state.StateMachine = null;
			connectedStates = FindConnectedStates().ToList();
			foreach (StateChip state in connectedStates)
			{
				state.StateMachine = this;
				if (object.ReferenceEquals(state, activeState))
					state.Active = true;
			}
		}

		private IEnumerable<StateChip> FindConnectedStates()
		{
			if (statePort.connections.Count == 0)
				yield break;
			else
				foreach (StateChip state in FindConnectedStates((StatePort)statePort.connections[0].TargetPort, new HashSet<StatePort> { statePort }))
					yield return state;
		}

		private IEnumerable<StateChip> FindConnectedStates(StatePort port, HashSet<StatePort> visited)
		{
			if (visited.Contains(port))
				yield break;
			DebugUtils.Assert(!port.IsStateRootPort);
			visited.Add(port);

			yield return (StateChip)port.node;

			foreach (StateMachineTransition connection in port.connections)
			{
				StatePort otherPort = connection.GetOtherPort(port);
				DebugUtils.Assert(otherPort != null);
				foreach (StateChip state in FindConnectedStates(otherPort, visited))
					yield return state;
			}
		}

		public StateChip NextStateAfterValidTransition()
		{
			StatePort port = activeState.statePort;
			foreach (StateMachineTransition transition in port.connections.Where(t => object.ReferenceEquals(t.SourcePort, port)).OrderBy(t => t.TargetPort.node))
			{
				DebugUtils.Assert(transition.TargetStatePort != null);
				if (ValueToBool(transition.TransitionEnabledPort.GetValue()))
					return (StateChip)transition.TargetStatePort.node;
			}
			return null;
		}

		public override IEnumerable<Connection> OutgoingConnections()
		{
			throw new System.NotImplementedException();
		}
	}
}
