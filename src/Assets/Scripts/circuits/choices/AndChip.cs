namespace AssemblyCSharp
{
	public class AndChip : Chip
	{
		public AndChip(CircuitManager manager) : base(manager, 7, 1, true)
		{
			foreach (InputPort port in inputPorts)
				port.UnconnectedValue = 1;
		}

		override public int IconIndex
		{
			get
			{
				return 15;
			}
		}

		override public void Evaluate()
		{
			outputPorts[0].Value = ToInt(
				!ToBool(inputPorts[7])
				&& (inputPorts[0].IsConnected
					|| inputPorts[1].IsConnected
					|| inputPorts[2].IsConnected
					|| inputPorts[3].IsConnected
					|| inputPorts[4].IsConnected
					|| inputPorts[5].IsConnected
					|| inputPorts[6].IsConnected)
				&& (ToBool(inputPorts[0])
					&& ToBool(inputPorts[1])
					&& ToBool(inputPorts[2])
					&& ToBool(inputPorts[3])
					&& ToBool(inputPorts[4])
					&& ToBool(inputPorts[5])
					&& ToBool(inputPorts[6])));
		}
	}
}