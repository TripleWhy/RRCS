namespace AssemblyCSharp
{
	using System;

	public class OutputPort : DataPort
	{
		private IConvertible value;
		public System.Type expectedType;

		public OutputPort(CircuitNode node, bool isReset)
			: base(node, false, isReset)
		{
		}

		public IConvertible Value
		{
			set
			{
				if (expectedType != null)
				{
					if (expectedType.IsPrimitive)
						DebugUtils.Assert(expectedType != null && value.GetType() == expectedType);
					else
						DebugUtils.Assert(expectedType == null || value.GetType() == expectedType);
				}
				if (value == this.value)
					return;
				this.value = value;
				EmitValueChanged();
			}
		}

		#region implemented abstract members of Port
		public override IConvertible GetInternalValue()
		{
			return value;
		}
		#endregion
	}
}
