namespace AssemblyCSharp
{
	using System;

	public class BitshiftLeftChip : SimpleCombinatorChip
	{
		public BitshiftLeftChip(CircuitManager manager)
			: base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 67;
			}
		}

		override protected IConvertible Combine(IConvertible a, IConvertible b)
		{
			if (a is long)
				return Convert.ToInt64(a) << Convert.ToInt32(b);
			else
				return Convert.ToInt32(a) << Convert.ToInt32(b);
		}
	}
}
