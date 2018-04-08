namespace AssemblyCSharp
{
	public abstract class SimpleCombinatorChip : Chip
	{
		protected SimpleCombinatorChip() : base(2, 1, true)
		{
		}

		override public void Evaluate()
		{
			if (ToBool(inputPorts[3].GetValue()))
				outputPorts[0].Value = 0;
			else
				outputPorts[0].Value = Combine(inputPorts[0].GetValue(), inputPorts[1].GetValue());
		}

		abstract protected int Combine(int a, int b);
	}
}

