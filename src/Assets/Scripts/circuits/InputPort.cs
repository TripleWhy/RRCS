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
				return connectedPorts[0].GetValue();
			else
				return UnconnectedValue;
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
