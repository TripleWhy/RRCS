﻿namespace AssemblyCSharp
{
	public abstract class CircuitNode
	{
		public readonly int inputPortCount;
		public readonly int outputPortCount;
		public readonly bool hasReset;
		public readonly InputPort[] inputPorts;
		public readonly OutputPort[] outputPorts;

		protected CircuitNode(int inputCount, int outputCount, bool hasReset)
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
		}

		public abstract void Evaluate();
	}
}