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
				return Convert.ToDouble(a) - Convert.ToDouble(b);
			if (a is float || b is float)
				return Convert.ToSingle(a) - Convert.ToSingle(b);
			if (a is long || b is long)
				return Convert.ToInt64(a) - Convert.ToInt64(b);
			if (a is int || b is int || a is bool || b is bool)
				return Convert.ToInt32(a) - Convert.ToInt32(b);
			return null;
		}
	}
}
