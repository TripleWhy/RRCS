namespace AssemblyCSharp
{
	using System;
	using System.Text;

	public class AddChip : Chip
	{
		public AddChip(CircuitManager manager)
			: base(manager, 3, 1, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 9;
			}
		}

		protected override IConvertible DefaultInputValue(int inputIndex)
		{
			return 0;
		}

		override protected void EvaluateOutputs()
		{
			outputPorts[0].Value = Add();
		}

		protected IConvertible Add()
		{
			IConvertible a = inputPorts[0].IsConnected ? InValue(0) : null;
			IConvertible b = inputPorts[1].IsConnected ? InValue(1) : null;
			IConvertible c = inputPorts[2].IsConnected ? InValue(2) : null;
			if (a is string || b is string || c is string)
			{
				inputPorts[0].UnconnectedValue = inputPorts[1].UnconnectedValue = inputPorts[2].UnconnectedValue = "";
				StringBuilder sb = new StringBuilder();
				sb.Append(InValue(0));
				sb.Append(InValue(1));
				sb.Append(InValue(2));
				return sb.ToString();
			}
			if (a is double || b is double || c is double)
			{
				inputPorts[0].UnconnectedValue = inputPorts[1].UnconnectedValue = inputPorts[2].UnconnectedValue = 0D;
				return Convert.ToDouble(InValue(0)) + Convert.ToDouble(InValue(1)) + Convert.ToDouble(InValue(2));
			}
			if (a is float || b is float || b is float)
			{
				inputPorts[0].UnconnectedValue = inputPorts[1].UnconnectedValue = inputPorts[2].UnconnectedValue = 0F;
				return Convert.ToSingle(InValue(0)) + Convert.ToSingle(InValue(1)) + Convert.ToSingle(InValue(2));
			}
			if (a is long || b is long || c is long)
			{
				inputPorts[0].UnconnectedValue = inputPorts[1].UnconnectedValue = inputPorts[2].UnconnectedValue = 0L;
				return Convert.ToInt64(InValue(0)) + Convert.ToInt64(InValue(1)) + Convert.ToInt64(InValue(2));
			}
			if ((a is int || b is int || c is int) || (a is bool || b is bool || c is bool))
			{
				inputPorts[0].UnconnectedValue = inputPorts[1].UnconnectedValue = inputPorts[2].UnconnectedValue = 0;
				return Convert.ToInt32(InValue(0)) + Convert.ToInt32(InValue(1)) + Convert.ToInt32(InValue(2));
			}
			else
			{
				inputPorts[0].UnconnectedValue = inputPorts[1].UnconnectedValue = inputPorts[2].UnconnectedValue = null;
				return null;
			}
		}
	}
}
