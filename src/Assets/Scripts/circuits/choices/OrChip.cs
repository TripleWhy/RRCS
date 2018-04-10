namespace AssemblyCSharp
{
	public class OrChip : Chip
	{
		public OrChip(CircuitManager manager) : base(manager, 7, 1, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 16;
			}
		}

		override public void Evaluate()
		{
			outputPorts[0].Value = ToInt(
				!ToBool(inputPorts[7])
				&& (ToBool(inputPorts[0])
				    || ToBool(inputPorts[1])
				    || ToBool(inputPorts[2])
				    || ToBool(inputPorts[3])
				    || ToBool(inputPorts[4])
				    || ToBool(inputPorts[5])
				    || ToBool(inputPorts[6])));
		}
	}
}