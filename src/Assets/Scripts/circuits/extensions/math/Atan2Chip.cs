namespace AssemblyCSharp
{
	using System;

	public class Atan2Chip : SimpleCombinatorChip
	{
		public Atan2Chip(CircuitManager manager)
			: base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 48;
			}
		}

		override protected IConvertible Combine(IConvertible a, IConvertible b)
		{
			return Math.Atan2(Convert.ToDouble(a), Convert.ToDouble(b));
		}
	}
}