namespace AssemblyCSharp
{
	using System.Collections.Generic;

	public class StatePort : Port
	{
		public enum StatePortType
		{
			None,
			Root,
			Node,
		};

		public int UnconnectedValue { get; set; }

		public readonly bool isRootPort = false;

		public StatePort(CircuitNode node, bool isRoot) : base(node, false)
		{
			isRootPort = isRoot;
		}

		public override int GetValue()
		{
			return -1;
		}

		public override bool IsInput
		{
			get
			{
				return false;
			}
		}

		public override bool IsState
		{
			get
			{
				return true;
			}
		}

		public List<StatePort> getAllConnectedRootPorts()
		{
			List<StatePort> found = new List<StatePort>();
			getAllConnectedRootPorts(new List<Port>(), found);
			return found;
		}

		private void getAllConnectedRootPorts(List<Port> checkedPorts, List<StatePort> foundPorts)
		{
			if (isRootPort)
				foundPorts.Add(this);
			checkedPorts.Add(this);

			foreach (Connection connection in connections)
			{
				var otherPort = connection.getOtherPort(this);
				if (otherPort != null && otherPort.IsState && !checkedPorts.Contains(otherPort))
				{
					((StatePort) otherPort).getAllConnectedRootPorts(checkedPorts, foundPorts);
				}
			}
		}

		public StateChip getConnectedActiveState()
		{
			return searchActiveState(new List<Port>());
		}

		private StateChip searchActiveState(List<Port> checkedPorts)
		{
			if (!isRootPort && ((StateChip) node).Active)
				return (StateChip) node;

			checkedPorts.Add(this);

			foreach (Connection connection in connections)
			{
				var otherPort = connection.getOtherPort(this);
				if (
					otherPort != null &&
					otherPort.IsState &&
					!((StatePort) otherPort).isRootPort &&
					!checkedPorts.Contains(otherPort)
				)
				{
					var active = ((StatePort) otherPort).searchActiveState(checkedPorts);
					if (active != null)
						return active;
				}
			}

			return null;
		}

		public StateChip getNextState()
		{
			DebugUtils.Assert(connections.Count <= 1);
			if (connections.Count == 1)
				return (StateChip) connections[0].targetPort.node;
			return null;
		}

		public StateChip getNextStateAfterValidTransition()
		{
			foreach (Connection connection in connections)
			{
				if (connection.sourcePort == this)
				{
					if (connection.targetPort != null && connection.targetPort.IsState &&
					    (!((StateMachineTransition) connection).transitionEnabledPort.IsConnected ||
					     ((StateMachineTransition) connection).transitionEnabledPort.GetValue() != 0))
					{
						return (StateChip) connection.targetPort.node;
					}
				}
			}

			return null;
		}

		public List<InputPort> getAllTransitionEnabledPorts()
		{
			List<InputPort> found = new List<InputPort>();
			searchTransitionEnabledPorts(new List<Port>(), found);
			return found;
		}

		private void searchTransitionEnabledPorts(List<Port> checkedPorts, List<InputPort> foundPorts)
		{
			checkedPorts.Add(this);

			foreach (var connection in connections)
			{
				var transition = (StateMachineTransition) connection;
				if (transition.sourcePort == this)
				{
					if (transition.transitionEnabledPort != null)
						foundPorts.Add(transition.transitionEnabledPort);
				}
				else if (transition.targetPort != null && transition.targetPort.IsState &&
				         !checkedPorts.Contains(transition.targetPort))
				{
					((StatePort) transition.targetPort).searchTransitionEnabledPorts(checkedPorts, foundPorts);
				}
			}
		}

		public StateMachineTransition[] getAllOutgoingTransitions()
		{
			List<StateMachineTransition> outgoing = new List<StateMachineTransition>();

			foreach (var connection in connections)
			{
				if (connection.sourcePort == this)
				{
					outgoing.Add((StateMachineTransition) connection);
				}
			}

			return outgoing.ToArray();
		}
	}
}