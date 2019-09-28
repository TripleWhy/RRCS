namespace AssemblyCSharp
{
	using System;
	using System.Collections;

	public class SimpleLessThanOrEqualChip : SimpleCombarerChipBase
	{
		public SimpleLessThanOrEqualChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 7;
			}
		}

		override protected bool Compare(IConvertible a, IConvertible b)
		{
			return Comparer.DefaultInvariant.Compare(a, b) <= 0;
		}
	}
}