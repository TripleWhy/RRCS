namespace AssemblyCSharp
{
	public class OutputPort : Port
	{
		public OutputPort(CircuitNode node, bool isReset) : base(node, isReset)
		{
		}

		public int Value { get; set; }

		#region implemented abstract members of Port
		public override int GetValue()
		{
			return Value;
		}

		public override bool IsInput
		{
			get
			{
				return false;
			}
		}
		#endregion
	}
}
