namespace AssemblyCSharp
{
	public class InputPort : Port
	{
		public int UnconnectedValue { get; set; }

		public InputPort(CircuitNode node, bool isReset) : base(node, isReset)
		{
		}

		#region implemented abstract members of Port
		public override int GetValue()
		{
			if (IsConnected)
				return connections[0].sourcePort.GetValue();
			else
				return UnconnectedValue;
		}

		public override bool IsState
		{
			get
			{
				return false;
			}
		}

		public override bool IsInput
		{
			get
			{
				return true;
			}
		}
		#endregion
	}
}
