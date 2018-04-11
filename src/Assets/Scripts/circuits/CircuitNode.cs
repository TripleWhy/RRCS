namespace AssemblyCSharp
{
	using System.Collections.Generic;

	public abstract class CircuitNode
	{
		public readonly int inputPortCount;
		public readonly int outputPortCount;
		public readonly bool hasReset;
		public readonly InputPort[] inputPorts;
		public readonly OutputPort[] outputPorts;
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
				inputPorts[i] = new InputPort(this, false);
			if (hasReset)
				inputPorts[inputCount] = new InputPort(this, true);
			
			int totalOutputCount = outputCount + (hasReset ? 1 : 0);
			outputPorts = new OutputPort[totalOutputCount];
			for (int i = 0; i < outputCount; i++)
				outputPorts[i] = new OutputPort(this, false);
			if (hasReset)
				outputPorts[outputCount] = new OutputPort(this, true);

			if (manager != null)
				manager.AddNode(this);
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
			if (hasReset && ToBool(inputPorts[inputPortCount]))
				outputPorts[outputPortCount].Value = 1;
			EvaluateOutputs();
		}

		protected abstract void EvaluateOutputs();
	}
}