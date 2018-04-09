﻿namespace AssemblyCSharp
{
	using System.Collections.Generic;
	
	public abstract class Port
	{
		public delegate void ConnectionEventHandler(Port sender, Port other);
		public event ConnectionEventHandler Connected = delegate {};
		public event ConnectionEventHandler Disconnected = delegate {};

		public readonly List<Port> connectedPorts = new List<Port>();
		public readonly CircuitNode node;
		public readonly bool isReset;

		protected Port(CircuitNode node, bool isReset)
		{
			this.node = node;
			this.isReset = isReset;
		}

		public abstract bool IsInput { get; }
		public abstract int GetValue();

		public bool IsConnected
		{
			get
			{
				return connectedPorts.Count != 0;
			}
		}

		public bool Connect(Port port)
		{
			if (port.IsInput == IsInput)
				return false;
			if (IsInput && IsConnected)
				return false;
			if (port.IsInput && port.IsConnected)
				return false;
			connectedPorts.Add(port);
			port.connectedPorts.Add(this);
			Connected(this, port);
			return true;
		}

		public void Disconnect(Port port)
		{
			if (!connectedPorts.Remove(port))
				return;
			port.connectedPorts.Remove(this);
			Disconnected(this, port);
		}
	}
}