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
				bool result = InBool(0)
					|| InBool(1)
					|| InBool(2)
					|| InBool(3)
					|| InBool(4)
					|| InBool(5)
					|| InBool(6);
				outputPorts[0].Value = result;
				outputPorts[1].Value = !result;
			}
		}
	}
}
