namespace AssemblyCSharp
{
	using System;

	public class SinChip : DoubleOperation1Arg
	{
		public SinChip(CircuitManager manager)
			: base(manager)
		{
		}

		public override int IconIndex
		{
			get
			{
				return 63;
			}
		}

		protected override double MathEval(double value)
		{
			return Math.Sin(value);
		}
	}
}