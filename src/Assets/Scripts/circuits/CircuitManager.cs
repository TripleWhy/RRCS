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
		}

		public void RemoveNode(CircuitNode node)
		{
			nodes.Remove(node);
			node.EvaluationRequired -= Node_EvaluationRequired;
		}

		private void Node_EvaluationRequired(CircuitNode source)
		{
			dirty = true;
		}

		public void EvaluateIfNecessary()
		{
			if (dirty)
			{
				dirty = false; //Clearing the flag before Evaluate() allows Evaluate() to set the flag again.
				Evaluate();
			}
		}

		public void Evaluate()
		{
			//TODO
		}
	}
}
