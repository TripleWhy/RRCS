namespace AssemblyCSharp
{
	public class NotChip : Chip
	{
		public NotChip(CircuitManager manager) : base(manager, 1, 1, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 17;
			}
		}

		override protected void EvaluateOutputs()
		{
			if (!inputPorts[0].IsConnected)
				outputPorts[0].Value = 0;
			else
				outputPorts[0].Value = ToInt(!ToBool(inputPorts[0]));
		}
	}
}