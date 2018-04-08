namespace AssemblyCSharp
{
	public class NotChip : Chip
	{
		public NotChip() : base(1, 1, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 17;
			}
		}

		override public void Evaluate()
		{
			outputPorts[0].Value = ToInt(!ToBool(inputPorts[1]) && !ToBool(inputPorts[0]));
		}
	}
}