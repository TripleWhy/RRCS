namespace AssemblyCSharp
{
	using System;

	public class BitshiftRightChip : SimpleCombinatorChip
	{
		public BitshiftRightChip(CircuitManager manager)
			: base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 68;
			}
		}

		override protected IConvertible Combine(IConvertible a, IConvertible b)
		{
			if (a is long)
				return ValueToLong(a) >> ValueToInt(b);
			else
				return ValueToInt(a) >> ValueToInt(b);
		}
	}
}
