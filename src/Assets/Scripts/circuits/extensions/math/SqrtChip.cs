namespace AssemblyCSharp
{
	using System;

	public class SqrtChip : DoubleOperation1Arg
	{
		public SqrtChip(CircuitManager manager)
			: base(manager)
		{
		}

		public override int IconIndex
		{
			get
			{
				return 64;
			}
		}

		protected override double MathEval(double value)
		{
			return Math.Sqrt(value);
		}
	}
}