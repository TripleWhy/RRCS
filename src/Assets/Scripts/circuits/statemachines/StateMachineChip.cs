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
				return (StateChip)statePort.connections[0].TargetPort.node;
			return null;
		}

		private bool IsActive
		{
			get
			{
				return !IsResetSet && !inputPorts[0].IsConnected || inputPorts[0].GetValue() != 0;
			}
		}

		public override void Evaluate()
		{
			DebugUtils.Assert(statePort != null);

			if (IsResetSet)
			{
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
			//TODO: Sort port.connections or make connections generally sorted.
			foreach (StateMachineTransition connection in port.connections)
			{
				if (object.ReferenceEquals(connection.SourcePort, port))
				{
					DebugUtils.Assert(connection.TargetStatePort != null);
					if (ToBool(connection.TransitionEnabledPort))
						return (StateChip)connection.TargetStatePort.node;
				}
			}
			return null;
		}

		public override IEnumerable<CircuitNode> SimpleDependsOn()
		{
			foreach (var port in FindTransitionEnabledPorts())
			{
				if (port.IsConnected && !ReferenceEquals(port.connections[0].SourcePort.node, this))
					yield return port.connections[0].SourcePort.node;
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
				if (object.ReferenceEquals(transition.SourcePort, port))
				{
					if (transition.TransitionEnabledPort != null)
						yield return transition.TransitionEnabledPort;
				}
				else
				{
					DebugUtils.Assert(transition.TargetPort != null);
					foreach (InputPort result in FindTransitionEnabledPorts(transition.TargetStatePort, visited))
						yield return result;
				}
			}
		}
	}
}
