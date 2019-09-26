namespace AssemblyCSharp
{
	using System;

	public class ModuloChip : SimpleCombinatorChip
	{
		public ModuloChip(CircuitManager manager)
			: base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 13;
			}
		}

		override protected IConvertible Combine(IConvertible a, IConvertible b)
		{
			if (a is double || b is double)
				return ValueToDouble(a) % ValueToDouble(b);
			if (a is float || b is float)
				return ValueToFloat(a) % ValueToFloat(b);
			if (a is long || b is long)
			{
				long lb = ValueToLong(b);
				if (lb == 0)
					return 0;
				return ValueToLong(a) % lb;
			}
			if (a is int || b is int || a is bool || b is bool)
			{
				int ib = ValueToInt(b);
				if (ib == 0)
					return 0;
				return ValueToInt(a) % ib;
			}
			return null;
		}
	}
}