namespace AssemblyCSharp
{
	using System;
	using System.Collections;

	public class SimpleLessThanChip : SimpleCombarerChipBase
	{
		public SimpleLessThanChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 6;
			}
		}

		override protected bool Compare(IConvertible a, IConvertible b)
		{
			return Comparer.DefaultInvariant.Compare(a, b) < 0;
		}
	}
}