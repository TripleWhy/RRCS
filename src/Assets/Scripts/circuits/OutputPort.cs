namespace AssemblyCSharp
{
	public class OutputPort : Port
	{
		private int value;

		public OutputPort(CircuitNode node, bool isReset) : base(node, isReset)
		{
		}

		public int Value
		{
			get
			{
				return value;
			}
			set
			{
				if (value == this.value)
					return;
				this.value = value;
				EmitValueChanged();
			}
		}

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
