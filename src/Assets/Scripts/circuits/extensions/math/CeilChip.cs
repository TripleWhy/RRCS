namespace AssemblyCSharp
{
	using System;

	public class CeilChip : DoubleOperation1Arg
	{
		public CeilChip(CircuitManager manager)
			: base(manager)
		{
		}

		public override int IconIndex
		{
			get
			{
				return 50;
			}
		}

		protected override double MathEval(double value)
		{
			return Math.Ceiling(value);
		}
	}
}