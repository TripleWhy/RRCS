namespace AssemblyCSharp
{
	using System;

	public class BitwiseXorChip : SimpleCombinatorChip
	{
		public BitwiseXorChip(CircuitManager manager)
			: base(manager)
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
				return ValueToLong(a) ^ ValueToLong(b);
			else
				return ValueToInt(a) ^ ValueToInt(b);
		}
	}
}
