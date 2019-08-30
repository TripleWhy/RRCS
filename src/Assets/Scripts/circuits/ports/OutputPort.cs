namespace AssemblyCSharp
{
	using System;

	public class OutputPort : DataPort
	{
		private IConvertible value;

		public OutputPort(CircuitNode node, bool isReset)
			: base(node, false, isReset)
		{
		}

		public IConvertible Value
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
		public override IConvertible GetValue()
		{
			return Value;
		}
		#endregion
	}
}
