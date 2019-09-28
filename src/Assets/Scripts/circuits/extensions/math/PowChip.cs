namespace AssemblyCSharp
{
	using System;

	public class PowChip : SimpleCombinatorChip
	{
		public PowChip(CircuitManager manager)
			: base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 60;
			}
		}

		override protected IConvertible Combine(IConvertible a, IConvertible b)
		{
			return Math.Pow(ValueToDouble(a), ValueToDouble(b));
		}
	}
}
