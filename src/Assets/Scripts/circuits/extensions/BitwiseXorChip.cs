namespace AssemblyCSharp
{
	using System;

	public class BitwiseXorChip : SimpleCombinatorChip
	{
		public BitwiseXorChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 41;
			}
		}

		override protected IConvertible Combine(IConvertible a, IConvertible b)
		{
			if (a is long || b is long)
				return Convert.ToInt64(a) ^ Convert.ToInt64(b);
			if (a is int || b is int)
				return Convert.ToInt32(a) ^ Convert.ToInt32(b);
			return 0;
		}
	}
}
