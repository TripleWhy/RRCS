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
		public readonly StatePort statePort;
		public readonly NodeSetting[] settings;
		internal int RingEvaluationPriority { get; set; }
		private CircuitManager manager;

		public delegate void EvaluationRequiredEventHandler(CircuitNode source);
		public event EvaluationRequiredEventHandler EvaluationRequired = delegate { };

		protected CircuitNode(CircuitManager manager, int inputCount, int outputCount, bool hasReset,
			StatePort.StatePortType statePortType = StatePort.StatePortType.None)
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
				inputPorts[i].Disconnected += CircuitNode_Connected;
			}
			if (hasReset)
			{
				inputPorts[inputCount] = new InputPort(this, true);
				inputPorts[inputCount].ValueChanged += CircuitNode_ValueChanged;
				inputPorts[inputCount].Connected += CircuitNode_Connected;
				inputPorts[inputCount].Disconnected += CircuitNode_Connected;
			}
			
			int totalOutputCount = outputCount + (hasReset ? 1 : 0);
			outputPorts = new OutputPort[totalOutputCount];
			for (int i = 0; i < outputCount; i++)
			{
				outputPorts[i] = new OutputPort(this, false);
				outputPorts[i].Connected += CircuitNode_Connected;
				outputPorts[i].Disconnected += CircuitNode_Connected;
			}
			if (hasReset)
			{
				outputPorts[outputCount] = new OutputPort(this, true);
				outputPorts[outputCount].Connected += CircuitNode_Connected;
				outputPorts[outputCount].Disconnected += CircuitNode_Connected;
			}

			switch (statePortType)
			{
				case StatePort.StatePortType.Root:
					statePort = new StatePort(this, true);
					break;
				case StatePort.StatePortType.Node:
					statePort = new StatePort(this, false);
					break;
				default:
					statePort = null;
					break;
			}

			Manager = manager;
		}

		~CircuitNode()
		{
			Destroy();
		}

		private void CircuitNode_Connected(Connection connection)
		{
			EmitEvaluationRequired();
		}

		private void CircuitNode_ValueChanged(Port sender)
		{
			EmitEvaluationRequired();
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
				statePort.Destroy();
			if (manager != null)
				manager.RemoveNode(this);
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

		public virtual IEnumerable<CircuitNode> DependsOn()
		{
			foreach (InputPort port in inputPorts)
			{
				if (port.IsConnected && !ReferenceEquals(port.connections[0].sourcePort.node, this))
					yield return port.connections[0].sourcePort.node;
			}
		}

		public static int ToInt(bool b)
		{
			return b ? 1 : 0;
		}

		public static bool ToBool(int i)
		{
			return i != 0;
		}

		public static int ToInt(Port p)
		{
			return p.GetValue();
		}

		public static bool ToBool(Port p)
		{
			return ToBool(ToInt(p));
		}

		protected int InValue(int index)
		{
			return ToInt(inputPorts[index]);
		}

		protected bool InBool(int index)
		{
			return ToBool(inputPorts[index]);
		}

		protected bool IsResetSet
		{
			get
			{
				return InBool(inputPortCount);
			}
		}

		protected int ResetValue
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
			if (hasReset)
			{
				outputPorts[outputPortCount].Value = ResetValue;
				if (IsResetSet)
				{
					for (int i = 0; i < outputPortCount; ++i)
						outputPorts[i].Value = 0;
					return;
				}
			}
			EvaluateOutputs();
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
	}
}