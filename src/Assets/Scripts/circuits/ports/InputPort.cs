namespace AssemblyCSharp
{
	using System;

	public class InputPort : DataPort
	{
		public IConvertible UnconnectedValue { get; set; }
		public IConvertible Value { get; private set; }
		private StateChip stateChip;

		public InputPort(CircuitNode node, bool isReset)
			: base(node, true, isReset)
		{
			stateChip = node as StateChip;
		}

		public override CircuitNode Node
		{
			get
			{
				if (stateChip != null)
					return stateChip.StateMachine;
				return base.Node;
			}
		}

		public override IConvertible GetInternalValue()
		{
			return Value;
		}

		public void UpdateValue()
		{
			if (IsConnected)
				Value = ((DataConnection)connections[0]).SourceDataPort.GetValue();
			else
				Value = UnconnectedValue;
		}
	}
}
