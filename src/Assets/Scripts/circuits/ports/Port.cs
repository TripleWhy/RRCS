namespace AssemblyCSharp
{
	using System;
	using System.Collections.Generic;

	public abstract class Port
	{
		[Flags]
		public enum PortType
		{
			None                = 0x00,
			DataPort            = 0x01,
			DataInput           = 0x02 | DataPort,
			DataOutput          = 0x04 | DataPort,
			DataResetPort       = 0x08 | DataPort,
			DataResetInputPort  = DataResetPort | DataInput,
			DataResetOutputPort = DataResetPort | DataOutput,
			StatePort           = 0x100,
			StateRoot           = 0x200 | StatePort,
		};

		public delegate void ConnectionEventHandler(Connection connection);
		public event ConnectionEventHandler Connected = delegate {};
		public event ConnectionEventHandler Disconnected = delegate {};

		public readonly List<Connection> connections = new List<Connection>();

		public readonly CircuitNode node;
		public readonly PortType portType;

		protected Port(CircuitNode node, PortType portType)
		{
			this.node = node;
			this.portType = portType;
		}

		~Port()
		{
			Destroy();
		}

		public bool IsDataPort
		{
			get
			{
				return (portType & PortType.DataPort) == PortType.DataPort;
			}
		}

		public bool IsDataInput
		{
			get
			{
				return (portType & PortType.DataInput) == PortType.DataInput;
			}
		}

		public bool IsDataOutput
		{
			get
			{
				return (portType & PortType.DataOutput) == PortType.DataOutput;
			}
		}

		public bool IsResetPort
		{
			get
			{
				return (portType & PortType.DataResetPort) == PortType.DataResetPort;
			}
		}

		public bool IsStatePort
		{
			get
			{
				return (portType & PortType.StatePort) == PortType.StatePort;
			}
		}

		public bool IsStateRootPort
		{
			get
			{
				return (portType & PortType.StateRoot) == PortType.StateRoot;
			}
		}

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

		public abstract bool Connect(Port port);

		public void Disconnect(Connection connection)
		{
			if (connections.Remove(connection))
				Disconnected(connection);
		}

		public void DisconnectAll()
		{
			for (int i = connections.Count - 1; i >= 0; i--)
				connections[i].Disconnect();

			DebugUtils.Assert(connections.Count == 0);
		}

		protected void EmitConnected(Connection connection)
		{
			Connected(connection);
		}

		protected void EmitDisconnected(Connection connection)
		{
			Disconnected(connection);
		}
	}
}