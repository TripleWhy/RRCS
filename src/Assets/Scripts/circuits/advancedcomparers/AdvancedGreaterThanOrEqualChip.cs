namespace AssemblyCSharp
{
	using System;
	using System.Collections;

	public class AdvancedGreaterThanOrEqualChip : AdvancedCombarerChipBase
	{
		public AdvancedGreaterThanOrEqualChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 5;
			}
		}

		override protected bool Compare(IConvertible a, IConvertible b)
		{
			return Comparer.DefaultInvariant.Compare(a, b) >= 0;
		}
	}
}