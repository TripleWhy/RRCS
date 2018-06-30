namespace AssemblyCSharp
{
	using UnityEngine;
	using System.Collections.Generic;

	static class UiManager
	{
		private static readonly Dictionary<CircuitNode, NodeUi> nodes = new Dictionary<CircuitNode, NodeUi>();
		private static readonly Dictionary<Port, PortUi> ports = new Dictionary<Port, PortUi>();
		private static bool showPortLabels = false;

		public static IEnumerable<MonoBehaviour> GetSelectables()
		{
			foreach (NodeUi ui in nodes.Values)
				yield return ui;
		}

		public static void Register(NodeUi nodeUi)
		{
			if (nodeUi.IsSidebarNode)
				return;
			Debug.Assert(!nodes.ContainsKey(nodeUi.Node));
			if (nodes.ContainsKey(nodeUi.Node))
				return;
			nodes[nodeUi.Node] = nodeUi;
			RRCSManager.Instance.selectionManager.SetSelectables(GetSelectables());
		}

		public static void Unregister(NodeUi nodeUi)
		{
			if (nodeUi.IsSidebarNode)
				return;
			nodes.Remove(nodeUi.Node);
			RRCSManager.Instance.selectionManager.SetSelectables(GetSelectables());
		}

		public static void Register(PortUi portUi)
		{
			if (portUi.nodeUi.IsSidebarNode)
				return;
			if (ports.ContainsKey(portUi.Port))
				return;
			ports.Add(portUi.Port, portUi);
			portUi.Port.Connected += Port_Connected;
			portUi.Port.Disconnected += Port_Disconnected;
			portUi.TextActive = showPortLabels;

			foreach (Port p in portUi.Port.connectedPorts)
				if (ports.ContainsKey(p))
					Port_Connected(portUi.Port, p);
		}

		public static void Unregister(PortUi portUi)
		{
			if (portUi.nodeUi == null || portUi.nodeUi.IsSidebarNode)
				return;
			portUi.Port.Connected -= Port_Connected;
			portUi.Port.Disconnected -= Port_Disconnected;

			foreach (Port p in portUi.Port.connectedPorts)
				if (ports.ContainsKey(p))
					Port_Disconnected(portUi.Port, p);

			ports.Remove(portUi.Port);
		}

		private static void Port_Connected(Port sender, Port other)
		{
			PortUi senderUi = GetUi(sender);
			PortUi otherUi = GetUi(other);
			LineRenderer line = senderUi.AddConnection(otherUi, null);
			otherUi.AddConnection(senderUi, line);
		}

		private static void Port_Disconnected(Port sender, Port other)
		{
			PortUi senderUi = GetUi(sender);
			PortUi otherUi = GetUi(other);
			bool destroy = senderUi.RemoveConnection(otherUi, false);
			otherUi.RemoveConnection(senderUi, destroy);
		}

		public static NodeUi GetUi(CircuitNode node)
		{
			return nodes[node];
		}

		public static ChipUi GetUi(Chip chip)
		{
			return (ChipUi)GetUi((CircuitNode)chip);
		}

		public static PortUi GetUi(Port port)
		{
			return ports[port];
		}

		public static ICollection<NodeUi> GetNodes()
		{
			return nodes.Values;
		}

		public static bool ShowPortLabels
		{
			get
			{
				return showPortLabels;
			}
			set
			{
				if (value == showPortLabels)
					return;
				showPortLabels = value;
				foreach (PortUi port in ports.Values)
					port.TextActive = value;
			}
		}
	}
}
