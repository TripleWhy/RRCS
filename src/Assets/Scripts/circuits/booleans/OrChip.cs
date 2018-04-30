namespace AssemblyCSharp
{
	public class OrChip : Chip
	{
		public OrChip(CircuitManager manager) : base(manager, 7, 2, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 16;
			}
		}

		override protected void EvaluateOutputs()
		{
			if (!(inputPorts[0].IsConnected
				|| inputPorts[1].IsConnected
				|| inputPorts[2].IsConnected
				|| inputPorts[3].IsConnected
				|| inputPorts[4].IsConnected
				|| inputPorts[5].IsConnected
				|| inputPorts[6].IsConnected))
				outputPorts[0].Value = outputPorts[1].Value = 0;
			else
			{
				outputPorts[0].Value = ToInt(
					ToBool(inputPorts[0])
						|| ToBool(inputPorts[1])
						|| ToBool(inputPorts[2])
						|| ToBool(inputPorts[3])
						|| ToBool(inputPorts[4])
						|| ToBool(inputPorts[5])
						|| ToBool(inputPorts[6]));
				outputPorts[1].Value = 1 - outputPorts[0].Value;
			}
		}
	}
}