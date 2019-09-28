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
				return InDouble(0) + InDouble(1) + InDouble(2);
			}
			if (a is float || b is float || b is float)
			{
				inputPorts[0].UnconnectedValue = inputPorts[1].UnconnectedValue = inputPorts[2].UnconnectedValue = 0F;
				return InFloat(0) + InFloat(1) + InFloat(2);
			}
			if (a is long || b is long || c is long)
			{
				inputPorts[0].UnconnectedValue = inputPorts[1].UnconnectedValue = inputPorts[2].UnconnectedValue = 0L;
				return InLong(0) + InLong(1) + InLong(2);
			}
			if ((a is int || b is int || c is int) || (a is bool || b is bool || c is bool))
			{
				inputPorts[0].UnconnectedValue = inputPorts[1].UnconnectedValue = inputPorts[2].UnconnectedValue = 0;
				return InInt(0) + InInt(1) + InInt(2);
			}
			else
			{
				inputPorts[0].UnconnectedValue = inputPorts[1].UnconnectedValue = inputPorts[2].UnconnectedValue = null;
				return null;
			}
		}
	}
}
