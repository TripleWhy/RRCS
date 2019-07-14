namespace AssemblyCSharp
{
	using UnityEngine;
	using System.Collections.Generic;

	static class UiManager
	{
		private static readonly Dictionary<CircuitNode, NodeUi> nodes = new Dictionary<CircuitNode, NodeUi>();
		private static readonly Dictionary<Port, PortUi> ports = new Dictionary<Port, PortUi>();
		private static readonly Dictionary<Connection, ConnectionUi> connections = new Dictionary<Connection, ConnectionUi>();
		private static bool showPortLabels = false;
		private static bool showOrderLabels = false;

		public static IEnumerable<MonoBehaviour> GetSelectables()
		{
			foreach (NodeUi ui in nodes.Values)
				yield return ui;
		}

		public static IEnumerable<GizmoUi> GetGizmos()
		{
			foreach (NodeUi node in nodes.Values)
			{
				GizmoUi gizmo = node as GizmoUi;
				if (gizmo != null)
					yield return gizmo;
			}
		}
		
		public static void Register(NodeUi nodeUi)
		{
			if (nodeUi.IsSidebarNode)
				return;
			DebugUtils.Assert(!nodes.ContainsKey(nodeUi.Node));
			if (nodes.ContainsKey(nodeUi.Node))
				return;
			nodes[nodeUi.Node] = nodeUi;
			RRCSManager.Instance.selectionManager.SetSelectables(GetSelectables());

			nodeUi.IndexTextActive = showOrderLabels;
			GizmoUi gimoUi = nodeUi as GizmoUi;
			if (gimoUi != null)
				gimoUi.TextActive = showPortLabels;
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
			if (portUi.nodeUi != null && portUi.nodeUi.IsSidebarNode)
				return;
			if (ports.ContainsKey(portUi.Port))
				return;
			ports.Add(portUi.Port, portUi);
			portUi.Port.Connected += Port_Connected;
			portUi.Port.Disconnected += Port_Disconnected;
			portUi.TextActive = showPortLabels;

			foreach (Connection c in portUi.Port.connections)
				if (ports.ContainsKey(c.sourcePort) && ports.ContainsKey(c.targetPort))
					Port_Connected(c);
		}

		public static void Unregister(PortUi portUi)
		{
			if (portUi.nodeUi == null || portUi.nodeUi.IsSidebarNode)
				return;
			portUi.Port.Connected -= Port_Connected;
			portUi.Port.Disconnected -= Port_Disconnected;

			foreach (Connection c in portUi.Port.connections)
				if (ports.ContainsKey(c.sourcePort) && ports.ContainsKey(c.targetPort))
					Port_Disconnected(c);

			ports.Remove(portUi.Port);
		}
		
		public static void Register(ConnectionUi connectionUi)
		{
			DebugUtils.Assert(!connections.ContainsKey(connectionUi.Connection));
			if (connections.ContainsKey(connectionUi.Connection))
				return;
			connections[connectionUi.Connection] = connectionUi;
		}

		public static void Unregister(ConnectionUi connectionUi)
		{
			if (connectionUi.Connection != null)
			{
				connections.Remove(connectionUi.Connection);
			}
		}

		private static void Port_Connected(Connection connection)
		{
			PortUi sourceUi = GetUi(connection.sourcePort);
			PortUi targetUi = GetUi(connection.targetPort);
			ConnectionUi connectionUi = sourceUi.AddConnection(sourceUi, targetUi, connection, null);
			targetUi.AddConnection(sourceUi, targetUi, connection, connectionUi);
		}

		private static void Port_Disconnected(Connection connection)
		{
			PortUi senderUi = GetUi(connection.sourcePort);
			PortUi otherUi = GetUi(connection.targetPort);
			bool destroy = senderUi.RemoveConnection(GetUi(connection), false);
			otherUi.RemoveConnection(GetUi(connection), destroy);
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

		public static ConnectionUi GetUi(Connection connection)
		{
			return connections[connection];
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
				foreach (GizmoUi node in GetGizmos())
					node.TextActive = value;
			}
		}

		public static bool ShowEvaluationOrderLabels
		{
			get
			{
				return showOrderLabels;
			}
			set
			{
				if (value == showOrderLabels)
					return;
				showOrderLabels = value;
				foreach (NodeUi node in nodes.Values)
					node.IndexTextActive = value;
			}
		}

		public static void ResetGizmos()
		{
			foreach (GizmoUi gizmo in GetGizmos())
				gizmo.Gizmo.reset();
		}
	}
}
