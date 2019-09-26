namespace AssemblyCSharp
{
	using System;
	using System.Text;

	public class MultiplyChip : SimpleCombinatorChip
	{
		public MultiplyChip(CircuitManager manager)
			: base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 11;
			}
		}

		protected override IConvertible DefaultOutputValue(int outputIndex)
		{
			return null;
		}

		override protected IConvertible Combine(IConvertible a, IConvertible b)
		{
			if (a is string && (b is int || b is long || b == null))
			{
				string str = (string)a;
				int count = ValueToInt(b);
				StringBuilder sb = new StringBuilder(str.Length * count);
				for (int i = 0; i < count; i++)
					sb.Append(str);
				return sb.ToString();
			}
			if (b is string && (a is int || a is long || a == null))
			{
				string str = (string)b;
				int count = ValueToInt(a);
				StringBuilder sb = new StringBuilder(str.Length * count);
				for (int i = 0; i < count; i++)
					sb.Append(str);
				return sb.ToString();
			}
			if (a is double || b is double)
				return ValueToDouble(a) * ValueToDouble(b);
			if (a is float || b is float)
				return ValueToFloat(a) * ValueToFloat(b);
			if (a is long || b is long)
				return ValueToLong(a) * ValueToLong(b);
			if (a is int || b is int || a is bool || b is bool)
				return ValueToInt(a) * ValueToInt(b);
			return null;
		}
	}
}
