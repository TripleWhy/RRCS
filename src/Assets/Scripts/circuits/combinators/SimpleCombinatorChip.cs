namespace AssemblyCSharp
{
	public abstract class SimpleCombinatorChip : Chip
	{
		protected SimpleCombinatorChip(CircuitManager manager) : base(manager, 2, 1, true)
		{
		}

		override protected void EvaluateOutputs()
		{
			outputPorts[0].Value = Combine(inputPorts[0].GetValue(), inputPorts[1].GetValue());
		}

		abstract protected int Combine(int a, int b);
	}
}

