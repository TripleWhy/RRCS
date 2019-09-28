namespace AssemblyCSharp
{
	using System;

	public class LogChip : SimpleCombinatorChip
	{
		public LogChip(CircuitManager manager)
			: base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 55;
			}
		}

		override protected IConvertible Combine(IConvertible a, IConvertible b)
		{
			return Math.Log(ValueToDouble(a), ValueToDouble(b));
		}
	}
}
