namespace AssemblyCSharp
{
	using System;

	public class TanChip : DoubleOperation1Arg
	{
		public TanChip(CircuitManager manager)
			: base(manager)
		{
		}

		public override int IconIndex
		{
			get
			{
				return 65;
			}
		}

		protected override double MathEval(double value)
		{
			return Math.Tan(value);
		}
	}
}