namespace AssemblyCSharp
{
	using System;

	public abstract class DoubleOperation1Arg : Chip
	{
		public DoubleOperation1Arg(CircuitManager manager)
			: base(manager, 1, 1, true)
		{
		}

		protected override Type ExpectedOutputType(int outputIndex)
		{
			return typeof(double);
		}

		protected override IConvertible DefaultInputValue(int inputIndex)
		{
			return 0d;
		}

		protected override IConvertible DefaultOutputValue(int outputIndex)
		{
			return 0d;
		}

		protected override void EvaluateOutputs()
		{
			outputPorts[0].Value = MathEval(Convert.ToDouble(InValue(0)));
		}

		protected abstract double MathEval(double value);
	}
}
