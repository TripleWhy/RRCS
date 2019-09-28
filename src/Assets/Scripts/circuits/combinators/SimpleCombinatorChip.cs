namespace AssemblyCSharp
{
	using System;

	public abstract class SimpleCombinatorChip : Chip
	{
		protected SimpleCombinatorChip(CircuitManager manager)
			: base(manager, 2, 1, true)
		{
		}

		protected override IConvertible DefaultInputValue(int inputIndex)
		{
			return 0;
		}

		protected override IConvertible DefaultOutputValue(int outputIndex)
		{
			return 0;
		}

		override protected void EvaluateOutputs()
		{
			outputPorts[0].Value = Combine(InValue(0), InValue(1));
		}

		abstract protected IConvertible Combine(IConvertible a, IConvertible b);
	}
}

