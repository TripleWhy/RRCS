namespace AssemblyCSharp
{
	using System;

	public class BitwiseAndChip : SimpleCombinatorChip
	{
		public BitwiseAndChip(CircuitManager manager)
			: base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 39;
			}
		}

		override protected IConvertible Combine(IConvertible a, IConvertible b)
		{
			if (a is long || b is long)
				return ValueToLong(a) & ValueToLong(b);
			else
				return ValueToInt(a) & ValueToInt(b);
		}
	}
}
