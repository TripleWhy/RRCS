namespace AssemblyCSharp
{
	using System;

	public class ModuloChip : SimpleCombinatorChip
	{
		public ModuloChip(CircuitManager manager) : base(manager)
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
				return Convert.ToDouble(a) % Convert.ToDouble(b);
			if (a is float || b is float)
				return Convert.ToSingle(a) % Convert.ToSingle(b);
			if (a is long || b is long)
			{
				long lb = Convert.ToInt64(b);
				if (lb == 0)
					return 0;
				return Convert.ToInt64(a) % lb;
			}
			if (a is int || b is int)
			{
				int ib = Convert.ToInt32(b);
				if (ib == 0)
					return 0;
				return Convert.ToInt32(a) % ib;
			}
			return null;
		}
	}
}