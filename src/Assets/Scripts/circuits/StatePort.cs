using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
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
		
		public List<Port> getAllConnectedRootPorts()
		{
			List<Port> found = new List<Port>();
			getAllConnectedRootPorts(new List<Port>(), found);
			return found;
		}
        
		private void getAllConnectedRootPorts(List<Port> checkedPorts, List<Port> foundPorts)
		{
            if (isRootPort)
	            foundPorts.Add(this);
			checkedPorts.Add(this);

			foreach (Connection connection in connections)
			{
				var otherPort = connection.getOtherPort(this);
				if (otherPort != null && otherPort.IsState && !checkedPorts.Contains(otherPort))
				{
					((StatePort)otherPort).getAllConnectedRootPorts(checkedPorts, foundPorts);
				}
			}
		}
	}
}
