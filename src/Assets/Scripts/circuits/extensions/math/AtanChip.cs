namespace AssemblyCSharp
{
	using System;

	public class AtanChip : DoubleOperation1Arg
	{
		public AtanChip(CircuitManager manager)
			: base(manager)
		{
		}

		public override int IconIndex
		{
			get
			{
				return 47;
			}
		}

		protected override double MathEval(double value)
		{
			return Math.Atan(value);
		}
	}
}