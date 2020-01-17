namespace AssemblyCSharp
{
	using System;

	public class InputPort : DataPort
	{
		public IConvertible UnconnectedValue { get; set; }
		public IConvertible Value { get; private set; }

		public InputPort(CircuitNode node, bool isReset)
			: base(node, true, isReset)
		{
		}

		#region implemented abstract members of Port
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
		#endregion
	}
}
