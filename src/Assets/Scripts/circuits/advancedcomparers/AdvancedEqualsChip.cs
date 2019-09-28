namespace AssemblyCSharp
{
	using System;
	using System.Collections;

	public class AdvancedEqualsChip : AdvancedCombarerChipBase
	{
		public AdvancedEqualsChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 2;
			}
		}

		override protected bool Compare(IConvertible a, IConvertible b)
		{
			return Comparer.DefaultInvariant.Compare(a, b) == 0;
		}
	}
}