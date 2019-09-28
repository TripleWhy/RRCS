namespace AssemblyCSharp
{
	using System;
	using System.Collections;

	public class SimpleNotEqualsChip : SimpleCombarerChipBase
	{
		public SimpleNotEqualsChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 3;
			}
		}

		override protected bool Compare(IConvertible a, IConvertible b)
		{
			return Comparer.DefaultInvariant.Compare(a, b) != 0;
		}
	}
}