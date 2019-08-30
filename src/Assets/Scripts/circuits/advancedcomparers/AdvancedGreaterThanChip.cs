﻿namespace AssemblyCSharp
{
	using System;
	using System.Collections;

	public class AdvancedGreaterThanChip : AdvancedCombarerChipBase
	{
		public AdvancedGreaterThanChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 4;
			}
		}

		override protected bool Compare(IConvertible a, IConvertible b)
		{
			return Comparer.DefaultInvariant.Compare(a, b) > 0;
		}
	}
}