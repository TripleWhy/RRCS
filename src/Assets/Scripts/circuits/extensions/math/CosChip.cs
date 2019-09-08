namespace AssemblyCSharp
{
	using System;

	public class CosChip : DoubleOperation1Arg
	{
		public CosChip(CircuitManager manager)
			: base(manager)
		{
		}

		public override int IconIndex
		{
			get
			{
				return 52;
			}
		}

		protected override double MathEval(double value)
		{
			return Math.Cos(value);
		}
	}
}