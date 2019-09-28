namespace AssemblyCSharp
{
	using System;

	public class RoundChip : DoubleOperation1Arg
	{
		public RoundChip(CircuitManager manager)
			: base(manager)
		{
		}

		public override int IconIndex
		{
			get
			{
				return 61;
			}
		}

		protected override double MathEval(double value)
		{
			return Math.Round(value);
		}
	}
}