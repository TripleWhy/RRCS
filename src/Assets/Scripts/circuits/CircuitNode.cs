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
		internal int RingEvaluationPriority { get; set; }
		protected readonly CircuitManager manager;

		public delegate void EvaluationRequiredEventHandler(CircuitNode source);
		public event EvaluationRequiredEventHandler EvaluationRequired = delegate { };

		protected CircuitNode(CircuitManager manager, int inputCount, int outputCount, bool hasReset)
		{
			this.inputPortCount = inputCount;
			this.outputPortCount = outputCount;
			this.hasReset = hasReset;
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

			if (manager != null)
				manager.AddNode(this);
		}

		private void CircuitNode_Connected(Port sender, Port other)
		{
			EvaluationRequired(this);
		}

		private void CircuitNode_ValueChanged(Port sender)
		{
			EvaluationRequired(this);
		}

		~CircuitNode()
		{
			Destroy();
		}

		public virtual void Destroy()
		{
			foreach (OutputPort port in outputPorts)
				port.Destroy();
			foreach (InputPort port in inputPorts)
				port.Destroy();
			manager.RemoveNode(this);
		}

		public IEnumerable<CircuitNode> DependsOn()
		{
			foreach (InputPort port in inputPorts)
			{
				if (port.IsConnected && !object.ReferenceEquals(port.connectedPorts[0].node, this))
					yield return port.connectedPorts[0].node;
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

		public virtual void Evaluate()
		{
			if (hasReset)
			{
				outputPorts[outputPortCount].Value = inputPorts[inputPortCount].GetValue();
				if (ToBool(outputPorts[outputPortCount]))
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
	}
}