namespace AssemblyCSharp
{
	using System;

	public class InputPort : DataPort
	{
		public IConvertible UnconnectedValue { get; set; }

		public InputPort(CircuitNode node, bool isReset)
			: base(node, true, isReset)
		{
		}

		#region implemented abstract members of Port
		public override IConvertible GetValue()
		{
			if (IsConnected)
				return ((DataConnection)connections[0]).SourceDataPort.GetValue();
			else
				return UnconnectedValue;
		}
		#endregion
	}
}
