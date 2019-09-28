namespace AssemblyCSharp
{
	using System;

	public class ValueStoreChip : Chip
	{
		private IConvertible value = null;

		public ValueStoreChip(CircuitManager manager)
			: base(manager, 2, 1, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 70;
			}
		}

		protected override IConvertible DefaultInputValue(int inputIndex)
		{
			if (inputIndex == 1)
				return true;
			return base.DefaultInputValue(inputIndex);
		}

		protected override void Reset()
		{
			value = null;
			base.Reset();
		}

		override protected void EvaluateOutputs()
		{
			inputPorts[1].UnconnectedValue = InBool(0);
			if (InBool(1))
				value = InValue(0);
			outputPorts[0].Value = value;
		}
	}
}
