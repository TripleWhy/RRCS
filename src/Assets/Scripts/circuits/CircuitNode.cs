namespace AssemblyCSharp
{
	using System;
	using System.Collections.Generic;

	public abstract class CircuitNode : IComparable<CircuitNode>
	{
		public readonly int inputPortCount;
		public readonly int outputPortCount;
		public readonly bool hasReset;
		public readonly InputPort[] inputPorts;
		public readonly OutputPort[] outputPorts;
		public StatePort statePort;
		public readonly NodeSetting[] settings;
		private int ringEvaluationPriority;
		private CircuitManager manager;

		public delegate void CircuitNodeChangedEventHandler(CircuitNode source);
		public event CircuitNodeChangedEventHandler EvaluationRequired = delegate { };
		public event CircuitNodeChangedEventHandler ConnectionChanged = delegate { };
		public event CircuitNodeChangedEventHandler RingEvaluationPriorityChanged = delegate { };

		protected CircuitNode(CircuitManager manager, int inputCount, int outputCount, bool hasReset, Port.PortType statePortType = StatePort.PortType.None)
		{
			this.inputPortCount = inputCount;
			this.outputPortCount = outputCount;
			this.hasReset = hasReset;
			this.settings = CreateSettings();
			int totalInputCount = inputCount + (hasReset ? 1 : 0);
			inputPorts = new InputPort[totalInputCount];
			for (int i = 0; i < inputCount; i++)
			{
				inputPorts[i] = new InputPort(this, false);
				inputPorts[i].ValueChanged += CircuitNode_ValueChanged;
				inputPorts[i].Connected += CircuitNode_Connected;
				inputPorts[i].Disconnected += CircuitNode_Disconnected;
			}
			if (hasReset)
			{
				inputPorts[inputCount] = new InputPort(this, true);
				inputPorts[inputCount].ValueChanged += CircuitNode_ValueChanged;
				inputPorts[inputCount].Connected += CircuitNode_Connected;
				inputPorts[inputCount].Disconnected += CircuitNode_Disconnected;
			}
			
			int totalOutputCount = outputCount + (hasReset ? 1 : 0);
			outputPorts = new OutputPort[totalOutputCount];
			for (int i = 0; i < outputCount; i++)
			{
				outputPorts[i] = new OutputPort(this, false);
				outputPorts[i].Connected += CircuitNode_Connected;
				outputPorts[i].Disconnected += CircuitNode_Disconnected;
			}
			if (hasReset)
			{
				outputPorts[outputCount] = new OutputPort(this, true);
				outputPorts[outputCount].Connected += CircuitNode_Connected;
				outputPorts[outputCount].Disconnected += CircuitNode_Disconnected;
			}

			if ((statePortType & Port.PortType.StatePort) != 0)
			{
				statePort = new StatePort(this, (statePortType & Port.PortType.StateRoot) == Port.PortType.StateRoot);
			}

			Manager = manager;
		}

		~CircuitNode()
		{
			Destroy();
		}

		public virtual void Destroy()
		{
			if (RingEvaluationPriority < 0)
				return;
			foreach (OutputPort port in outputPorts)
				port.Destroy();
			foreach (InputPort port in inputPorts)
				port.Destroy();
			if (statePort != null)
			{
				statePort.Destroy();
				statePort = null;
			}
			if (manager != null)
				manager.RemoveNode(this);
		}

		private void CircuitNode_Connected(Connection connection)
		{
			DebugUtils.Assert(connection.TargetPort.IsDataInput);
			EmitConnectionChanged();
			EmitEvaluationRequired();
		}

		private void CircuitNode_Disconnected(Connection connection)
		{
			EmitConnectionChanged();
			EmitEvaluationRequired();
		}

		private void CircuitNode_ValueChanged(Port sender)
		{
			EmitEvaluationRequired();
		}

		public CircuitManager Manager
		{
			get
			{
				return manager;
			}
			set
			{
				if (object.ReferenceEquals(value, manager))
					return;
				if (manager != null)
					throw new InvalidOperationException("CircuitManager already assigned.");
				manager = value;
				if (manager != null)
					manager.AddNode(this);
			}
		}

		public int RingEvaluationPriority
		{
			get
			{
				return ringEvaluationPriority;
			}
			internal set
			{
				ringEvaluationPriority = value;
				EmitRingEvaluationPriorityChanged();
			}
		}

		public SortedSet<CircuitNode> DependsOn()
		{
			SortedSet<CircuitNode> dependencies = new SortedSet<CircuitNode>(SimpleDependsOn());
			return dependencies;
		}

		public void DependsOn(SortedSet<CircuitNode> depedencies)
		{
			depedencies.Clear();
			depedencies.UnionWith(SimpleDependsOn());
		}

		public virtual IEnumerable<CircuitNode> SimpleDependsOn()
		{
			foreach (Connection connection in IncomingConnections())
				yield return connection.SourcePort.node;
		}

		public SortedSet<CircuitNode> DependingOnThis()
		{
			SortedSet<CircuitNode> dependants = new SortedSet<CircuitNode>(SimpleDependingOnThis());
			return dependants;
		}

		public void DependingOnThis(SortedSet<CircuitNode> dependants)
		{
			dependants.Clear();
			dependants.UnionWith(SimpleDependingOnThis());
		}

		public virtual IEnumerable<CircuitNode> SimpleDependingOnThis()
		{
			foreach (Connection connection in OutgoingConnections())
				yield return connection.TargetPort.node;
		}

		public virtual IEnumerable<Connection> IncomingConnections()
		{
			foreach (InputPort port in inputPorts)
				if (port.IsConnected && !ReferenceEquals(port.connections[0].SourcePort.node, this))
					yield return port.connections[0];
		}

		public virtual IEnumerable<Connection> OutgoingConnections()
		{
			foreach (OutputPort port in outputPorts)
				foreach (Connection connection in port.connections)
					if (!ReferenceEquals(connection.TargetPort.node, this))
						yield return connection;
		}

		public static int ValueToInt(IConvertible val)
		{
			return Convert.ToInt32(val);
		}

		public static float ValueToFloat(IConvertible val)
		{
			return Convert.ToSingle(val);
		}

		public static bool ValueToBool(IConvertible val)
		{
			return Convert.ToBoolean(val);
		}

		protected IConvertible InValue(int index)
		{
			return inputPorts[index].GetValue();
		}

		protected bool InBool(int index)
		{
			return ValueToBool(InValue(index));
		}

		protected int InInt(int index)
		{
			return ValueToInt(InValue(index));
		}

		protected float InFloat(int index)
		{
			return ValueToFloat(InValue(index));
		}

		protected bool IsResetSet
		{
			get
			{
				return InBool(inputPortCount);
			}
		}

		protected IConvertible ResetValue
		{
			get
			{
				return InValue(inputPortCount);
			}
		}

		public virtual void Tick()
		{
		}

		public virtual void Evaluate()
		{
			if (!hasReset)
				EvaluateOutputs();
			else if (!IsResetSet)
			{
				EvaluateOutputs();
				outputPorts[outputPortCount].Value = false;
			}
			else
			{
				outputPorts[outputPortCount].Value = ResetValue;
				for (int i = 0; i < outputPortCount; ++i)
					outputPorts[i].Value = 0;
			}
		}

		protected abstract void EvaluateOutputs();

		public int CompareTo(CircuitNode other)
		{
			return RingEvaluationPriority.CompareTo(other.RingEvaluationPriority);
		}

		protected virtual NodeSetting[] CreateSettings()
		{
			return new NodeSetting[0];
		}

		public virtual void SetSetting(NodeSetting setting, object value)
		{
			if (value == setting.currentValue)
				return;
			setting.currentValue = value;
			EmitEvaluationRequired();
		}

		public void SetSetting(NodeSetting.SettingType type, object value)
		{
			foreach (NodeSetting setting in settings)
			{
				if (setting.type == type)
				{
					SetSetting(setting, value);
					break;
				}
			}
		}

		protected void EmitEvaluationRequired()
		{
			EvaluationRequired(this);
		}

		protected void EmitConnectionChanged()
		{
			ConnectionChanged(this);
		}

		protected void EmitRingEvaluationPriorityChanged()
		{
			RingEvaluationPriorityChanged(this);
		}
	}
}