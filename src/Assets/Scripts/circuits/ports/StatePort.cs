namespace AssemblyCSharp
{
	using System.Collections.Generic;

	public class StatePort : Port
	{
		public StatePort(CircuitNode node, bool isRoot) : base(node, isRoot ? PortType.StateRoot : PortType.StatePort)
		{
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
				EmitDisconnected(existingTransition);
				return true;
			}

			StatePort targetRootPort;
			StatePort sourceRootPort;
			{
				HashSet<StatePort> visited = new HashSet<StatePort>();
				targetRootPort = target.FindConnectedRootPort(visited);
				visited.Clear();
				sourceRootPort = source.FindConnectedRootPort(visited);
			}

			if (sourceRootPort != null && targetRootPort != null && !object.ReferenceEquals(sourceRootPort, targetRootPort))
				return false;

			if (source.IsStateRootPort)
			{
				// Only one connection from a root state is allowed at a time
				for (int i = source.connections.Count - 1; i >= 0; i--)
				{
					var connection = source.connections[i];
					connection.Disconnect();
					EmitDisconnected(connection);
				}
			}

			StateMachineTransition transition = new StateMachineTransition(source, target);
			connections.Add(transition);
			port.connections.Add(transition);
			EmitConnected(transition);
			return true;
		}

		public StatePort FindConnectedRootPort()
		{
			return FindConnectedRootPort(new HashSet<StatePort>());
		}

		public StatePort FindConnectedRootPort(HashSet<StatePort> visited)
		{
			if (IsStateRootPort)
				return this;
			visited.Add(this);

			foreach (StateMachineTransition connection in connections)
			{
				StatePort otherPort = connection.GetOtherPort(this);
				if (otherPort != null && otherPort.IsStatePort && !visited.Contains(otherPort))
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
				if (object.ReferenceEquals(connection.SourcePort, this))
					yield return connection;
		}
	}
}
