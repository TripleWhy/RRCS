using System.Diagnostics;

namespace AssemblyCSharp
{
	using System.Collections.Generic;

	public abstract class Port
	{
		public delegate void ConnectionEventHandler(Connection connection);

		public delegate void ValueChangedEventHandler(Port sender);

		public event ConnectionEventHandler Connected = delegate { };
		public event ConnectionEventHandler Disconnected = delegate { };
		public event ValueChangedEventHandler ValueChanged = delegate { };

		public readonly List<Connection> connections = new List<Connection>();

		public readonly CircuitNode node;
		public readonly bool isReset;

		protected Port(CircuitNode node, bool isReset)
		{
			this.node = node;
			this.isReset = isReset;
		}

		~Port()
		{
			Destroy();
		}

		public abstract bool IsInput { get; }
		public abstract int GetValue();

		public abstract bool IsState { get; }

		public bool IsConnected
		{
			get
			{
				return connections.Count != 0;
			}
		}

		public void Destroy()
		{
			DisconnectAll();
		}

		public bool Connect(Port port)
		{
			if (IsState || port.IsState)
			{
				if (!IsState || !port.IsState)
					return false;

				StatePort source = (StatePort) this;
				StatePort target = (StatePort) port;

				if (target.isRootPort)
				{
					StatePort swap = target;
					target = source;
					source = swap;
				}

				StateMachineTransition transition =
					new StateMachineTransition(source, target, new InputPort(null, false));
				Connection existingTransition = connections.Find(t => t.connectsSamePorts(transition));
				if (existingTransition != null)
				{
					existingTransition.Disconnect();
					Disconnected(existingTransition);
					return true;
				}

				var targetRootPorts = target.getAllConnectedRootPorts();
				var sourceRootPorts = source.getAllConnectedRootPorts();
				Debug.Assert(targetRootPorts.Count <= 1);
				Debug.Assert(sourceRootPorts.Count <= 1);

				// Only one StateChip is allowed in a network
				if (sourceRootPorts.Count == 1 &&
				    targetRootPorts.Count == 1 &&
				    sourceRootPorts[0] != targetRootPorts[0])
					return false;

				if (source.isRootPort)
				{
					// Only one connection from a root state is allowed at a time
					for (int i = source.connections.Count - 1; i >= 0; i--)
					{
						var connection = source.connections[i];
						connection.Disconnect();
						Disconnected(connection);
					}
				}

				connections.Add(transition);
				port.connections.Add(transition);
				Connected(transition);
				return true;
			}
			else
			{
				if (port.IsInput == IsInput)
					return false;
				if (IsInput && IsConnected)
					return false;
				if (port.IsInput && port.IsConnected)
					return false;
				if (IsInput)
					port.ValueChanged += ValueChanged;
				else //port.IsInput
					ValueChanged += port.ValueChanged;

				Connection connection;
				if (IsInput)
					connection = new Connection(port, this);
				else
					connection = new Connection(this, port);

				connections.Add(connection);
				port.connections.Add(connection);
				Connected(connection);
				return true;
			}
		}

		public void Disconnect(Connection connection)
		{
			if (connections.Remove(connection))
				Disconnected(connection);
		}

		public void DisconnectAll()
		{
			for (int i = connections.Count - 1; i >= 0; i--)
				Disconnect(connections[i]);

			Debug.Assert(connections.Count == 0);
		}

		protected void EmitValueChanged()
		{
			ValueChanged(this);
		}
	}
}