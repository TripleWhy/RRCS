namespace AssemblyCSharp
{
	using System.Collections.Generic;

	public class CircuitManager
	{
		private readonly HashSet<CircuitNode> nodes = new HashSet<CircuitNode>();
		private bool dirty = true;

		public void AddNode(CircuitNode node)
		{
			if (nodes.Contains(node))
				return;
			nodes.Add(node);
			node.EvaluationRequired += Node_EvaluationRequired;
			foreach (Port port in node.inputPorts)
			{
				port.Connected += Invalidate;
				port.Disconnected += Invalidate;
			}
			foreach (Port port in node.outputPorts)
			{
				port.Connected += Invalidate;
				port.Disconnected += Invalidate;
			}
			Invalidate();
		}

		public void RemoveNode(CircuitNode node)
		{
			nodes.Remove(node);
			node.EvaluationRequired -= Node_EvaluationRequired;
			foreach (Port port in node.inputPorts)
			{
				port.Connected += Invalidate;
				port.Disconnected += Invalidate;
			}
			foreach (Port port in node.outputPorts)
			{
				port.Connected += Invalidate;
				port.Disconnected += Invalidate;
			}
			Invalidate();
		}

		private void Invalidate(Port sender, Port other)
		{
			Invalidate();
		}

		public void Invalidate()
		{
			dirty = true;
		}

		private void Node_EvaluationRequired(CircuitNode source)
		{
			Invalidate();
		}

		public void EvaluateIfNecessary()
		{
			dirty = true; //TODO: Remove and instead firgure out a way to allow self-modifying loops to set the flag.
			if (dirty)
			{
				dirty = false; //Clearing the flag before Evaluate() allows Evaluate() to set the flag again.
				Evaluate();
			}
		}

		public void Evaluate()
		{
			foreach (CircuitNode node in nodes)
				node.Evaluate();
		}
	}
}
