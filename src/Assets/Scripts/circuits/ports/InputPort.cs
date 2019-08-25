namespace AssemblyCSharp
{
	public class InputPort : DataPort
	{
		public int UnconnectedValue { get; set; }

		public InputPort(CircuitNode node, bool isReset)
			: base(node, true, isReset)
		{
		}

		#region implemented abstract members of Port
		public override int GetValue()
		{
			if (IsConnected)
				return ((DataConnection)connections[0]).SourceDataPort.GetValue();
			else
				return UnconnectedValue;
		}
		#endregion
	}
}
