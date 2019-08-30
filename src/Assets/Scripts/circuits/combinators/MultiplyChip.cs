namespace AssemblyCSharp
{
	using System;
	using System.Text;

	public class MultiplyChip : SimpleCombinatorChip
	{
		public MultiplyChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 11;
			}
		}

		override protected IConvertible Combine(IConvertible a, IConvertible b)
		{
			if (a is double || b is double)
				return Convert.ToDouble(a) * Convert.ToDouble(b);
			if (a is float || b is float)
				return Convert.ToSingle(a) * Convert.ToSingle(b);
			if (a is long || b is long)
				return Convert.ToInt64(a) * Convert.ToInt64(b);
			if (a is int || b is int)
				return Convert.ToInt32(a) * Convert.ToInt32(b);
			if (a is string && (b is int || b is long || b == null))
			{
				string str = (string)b;
				int count = Convert.ToInt32(b);
				StringBuilder sb = new StringBuilder(str.Length * count);
				for (int i = 0; i < count; i++)
					sb.Append(str);
				return sb.ToString();
			}
			if (b is string && (a is int || a is long || a == null))
			{
				string str = (string)a;
				int count = Convert.ToInt32(a);
				StringBuilder sb = new StringBuilder(str.Length * count);
				for (int i = 0; i < count; i++)
					sb.Append(str);
				return sb.ToString();
			}
			return null;
		}
	}
}