namespace AssemblyCSharp
{
	using System;
	using System.Text;

	public class AddChip : Chip
	{
		public AddChip(CircuitManager manager) : base(manager, 3, 1, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 9;
			}
		}

		override protected void EvaluateOutputs()
		{
			outputPorts[0].Value = Add(InValue(0), InValue(1), InValue(2));
		}

		protected IConvertible Add(IConvertible a, IConvertible b, IConvertible c)
		{
			if (a is double || b is double || c is double)
				return Convert.ToDouble(a) + Convert.ToDouble(b) + Convert.ToDouble(c);
			if (a is float || b is float || b is float)
				return Convert.ToSingle(a) + Convert.ToSingle(b) + Convert.ToSingle(c);
			if (a is long || b is long ||c is long)
				return Convert.ToInt64(a) + Convert.ToInt64(b) + Convert.ToInt64(c);
			if (a is int || b is int || c is int)
				return Convert.ToInt32(a) + Convert.ToInt32(b) + Convert.ToInt32(c);
			if (a is string || b is string || c is string)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(a);
				sb.Append(b);
				sb.Append(c);
				return sb.ToString();
			}
			return null;
		}
	}
}