namespace AssemblyCSharp
{
	using UnityEngine;
	using System.Collections.Generic;

	static class UiManager
	{
		private static readonly Dictionary<Chip, ChipUi> chips = new Dictionary<Chip, ChipUi>();
		private static readonly Dictionary<Port, PortUi> ports = new Dictionary<Port, PortUi>();
		private static bool showPortLabels = false;

		public static void Register(ChipUi chipUi)
		{
			if (chipUi.IsSidebarChip)
				return;
			chips[chipUi.Chip] = chipUi;
		}

		public static void Unregister(ChipUi chipUi)
		{
			if (chipUi.IsSidebarChip)
				return;
			chips.Remove(chipUi.Chip);
		}

		public static void Register(PortUi portUi)
		{
			if (portUi.chipUi.IsSidebarChip)
				return;
			ports.Add(portUi.Port, portUi);
			portUi.Port.Connected += Port_Connected;
			portUi.Port.Disconnected += Port_Disconnected;
			portUi.TextActive = showPortLabels;
		}

		public static void Unregister(PortUi portUi)
		{
			if (portUi.chipUi == null || portUi.chipUi.IsSidebarChip)
				return;
			portUi.Port.Connected -= Port_Connected;
			portUi.Port.Disconnected -= Port_Disconnected;
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

		public static ChipUi GetUi(CircuitNode node)
		{
			Chip chip = node as Chip;
			if (chip != null)
				return GetUi(chip);
			else
				return null;
		}

		public static ChipUi GetUi(Chip chip)
		{
			return chips[chip];
		}

		public static PortUi GetUi(Port port)
		{
			return ports[port];
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
