namespace AssemblyCSharp
{
	using System;

	public class SubtractChip : SimpleCombinatorChip
	{
		public SubtractChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 10;
			}
		}

		override protected IConvertible Combine(IConvertible a, IConvertible b)
		{
			if (a is double || b is double)
				return ValueToDouble(a) - ValueToDouble(b);
			if (a is float || b is float)
				return ValueToFloat(a) - ValueToFloat(b);
			if (a is long || b is long)
				return ValueToLong(a) - ValueToLong(b);
			if (a is int || b is int || a is bool || b is bool)
				return ValueToInt(a) - ValueToInt(b);
			return null;
		}
	}
}
