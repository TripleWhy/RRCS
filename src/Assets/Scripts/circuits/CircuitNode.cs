namespace AssemblyCSharp
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;

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
			Debug.Assert(connection.targetPort.IsInput);
			connection.targetPort.node.MabeAlignPriority();
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

		private void MabeAlignPriority()
		{
			int dependsCount = DependsOn().Count();
			if (dependsCount == 1)
			{
				foreach (CircuitNode dep in DependsOn())
				{
					manager.UpdateNodePriority(this, dep.RingEvaluationPriority + 1);
					break;
				}
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