namespace AssemblyCSharp
{
	using System;

	public class ExpChip : DoubleOperation1Arg
	{
		public ExpChip(CircuitManager manager)
			: base(manager)
		{
		}

		public override int IconIndex
		{
			get
			{
				return 53;
			}
		}

		protected override double MathEval(double value)
		{
			return Math.Exp(value);
		}
	}
}