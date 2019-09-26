namespace AssemblyCSharp
{
	using System;

	public class BitwiseComplementChip : Chip
	{
		public BitwiseComplementChip(CircuitManager manager)
			: base(manager, 1, 1, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 69;
			}
		}

		protected override IConvertible DefaultInputValue(int inputIndex)
		{
			return 0;
		}

		protected override IConvertible DefaultOutputValue(int outputIndex)
		{
			return 0;
		}

		protected override void EvaluateOutputs()
		{
			IConvertible value = InValue(0);
			if (value is long)
				outputPorts[0].Value = ~ValueToLong(value);
			else
				outputPorts[0].Value = ~ValueToInt(value);
		}
	}
}
