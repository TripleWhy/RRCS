namespace AssemblyCSharp
{
	using System;

	public class FloorChip : DoubleOperation1Arg
	{
		public FloorChip(CircuitManager manager)
			: base(manager)
		{
		}

		public override int IconIndex
		{
			get
			{
				return 54;
			}
		}

		protected override double MathEval(double value)
		{
			return Math.Floor(value);
		}
	}
}