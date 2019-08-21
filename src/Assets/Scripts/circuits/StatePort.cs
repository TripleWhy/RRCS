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

		public StatePort FindConnectedRootPort()
		{
			return FindConnectedRootPort(new HashSet<StatePort>());
		}

		public StatePort FindConnectedRootPort(HashSet<StatePort> visited)
		{
			if (isRootPort)
				return this;
			visited.Add(this);

			foreach (Connection connection in connections)
			{
				StatePort otherPort = (StatePort)connection.getOtherPort(this);
				if (otherPort != null && otherPort.IsState && !visited.Contains(otherPort))
				{
					StatePort root = otherPort.FindConnectedRootPort(visited);
					if (root != null)
						return root;
				}
			}
			return null;
		}

		public IEnumerable<StateMachineTransition> GetOutgoingTransitions()
		{
			foreach (StateMachineTransition connection in connections)
				if (object.ReferenceEquals(connection.sourcePort, this))
					yield return connection;
		}
	}
}
