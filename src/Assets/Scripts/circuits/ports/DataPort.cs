namespace AssemblyCSharp
{
	using System;

	public abstract class DataPort : Port
	{
		public delegate void ValueChangedEventHandler(DataPort sender);
		public event ValueChangedEventHandler ValueChanged = delegate { };

		public DataPort(CircuitNode node, bool isInput, bool isReset)
			: base(node, isInput ? (isReset ? PortType.DataResetInputPort : PortType.DataInput) : (isReset ? PortType.DataResetOutputPort : PortType.DataOutput))
		{
		}

		public abstract IConvertible GetInternalValue();

		public IConvertible GetValue()
		{
			if (node.Manager.UseIntValuesOnly)
				return CircuitNode.ValueToInt(GetInternalValue());
			else
				return GetInternalValue();
		}

		public override bool Connect(Port port)
		{
			if (!port.IsDataPort)
				return false;
			if (port.IsDataInput == IsDataInput)
				return false;
			if (IsDataInput)
				return port.Connect(this);

			OutputPort source = (OutputPort)this;
			InputPort target = (InputPort)port;

			if (target.IsConnected)
				return false;

			source.ValueChanged += target.ValueChanged;
			Connection connection = new DataConnection(source, target);
			source.connections.Add(connection);
			target.connections.Add(connection);
			EmitConnected(connection);
			return true;
		}

		protected void EmitValueChanged()
		{
			ValueChanged(this);
		}
	}
}
