namespace AssemblyCSharp
{
	using System;

	public class BitwiseOrChip : SimpleCombinatorChip
	{
		public BitwiseOrChip(CircuitManager manager)
			: base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 40;
			}
		}

		override protected IConvertible Combine(IConvertible a, IConvertible b)
		{
			if (a is long || b is long)
				return Convert.ToInt64(a) | Convert.ToInt64(b);
			else
				return Convert.ToInt32(a) | Convert.ToInt32(b);
		}
	}
}
