namespace AssemblyCSharp
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	public class CircuitManager
	{
		private readonly List<CircuitNode> nodes = new List<CircuitNode>();
		private bool dirty = true;

		public void AddNode(CircuitNode node)
		{
			Debug.Assert(!nodes.Contains(node));
			if (nodes.Contains(node))
				return;

			node.RingEvaluationPriority = nodes.Count;
			nodes.Add(node);

			node.EvaluationRequired += Node_EvaluationRequired;
			Invalidate();
		}

		public void RemoveNode(CircuitNode node)
		{
			Debug.Assert(object.Equals(nodes[node.RingEvaluationPriority], node));
			nodes.RemoveAt(node.RingEvaluationPriority);
			for (int i = node.RingEvaluationPriority, s = nodes.Count; i < s; i++)
			{
				Debug.Assert(nodes[i].RingEvaluationPriority == i + 1);
				nodes[i].RingEvaluationPriority = i;
			}

			node.EvaluationRequired -= Node_EvaluationRequired;
			Invalidate();
		}

        public List<CircuitNode> GetNodes()
        {
            return nodes;
        }

		internal void UpdateNodePriority(CircuitNode node, int newPriority)
		{
			newPriority = Math.Min(Math.Max(newPriority, 0), nodes.Count - 1);
			int oldPriority = node.RingEvaluationPriority;
			Debug.Assert(object.Equals(nodes[oldPriority], node));
			if (newPriority == oldPriority)
				return;
			nodes.RemoveAt(oldPriority);
			nodes.Insert(newPriority, node);

			for (int i = Math.Min(oldPriority, newPriority), max = Math.Max(oldPriority, newPriority); i <= max; i++)
				nodes[i].RingEvaluationPriority = i;

			Debug.Assert(object.Equals(nodes[newPriority], node));
			Debug.Assert(node.RingEvaluationPriority == newPriority);
		}

		private void Invalidate(Port sender, Port other)
		{
			Invalidate();
		}

		public void Invalidate()
		{
			dirty = true;
		}

		internal void Clear()
		{
			foreach (CircuitNode node in nodes)
				node.EvaluationRequired -= Node_EvaluationRequired;
			nodes.Clear();
		}

		private void Node_EvaluationRequired(CircuitNode source)
		{
			Invalidate();
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
			//TODO this whole evaluation stuff seems overly complicated. Find a better solution?
			float t0 = Time.realtimeSinceStartup;

			HashSet<CircuitNode> pending = new HashSet<CircuitNode>();
			HashSet<CircuitNode> evaluated = new HashSet<CircuitNode>();
			List<CircuitNode> selected = new List<CircuitNode>();

			int loopRuns = 0;
			for (; loopRuns < 10000; loopRuns++)
			{
				Evaluate1(pending, evaluated);
				if (pending.Count == 0)
					break;
				while (true)
				{
					CircuitNode n = SelectPending(pending);
					selected.Add(n);
					evaluated.Add(n);
					int evaluatedCount = evaluated.Count;
					Evaluate1(pending, evaluated);
					if (evaluated.Count > evaluatedCount)
						break;
				}
				for (int i = selected.Count - 1; i >= 0; --i)
					evaluated.Remove(selected[i]);
				selected.Clear();
			}

			float t1 = Time.realtimeSinceStartup;
			Debug.Log("Time: " + (t1 - t0) + " loops: " + (loopRuns + 1));

			Debug.Assert(pending.Count == 0);
		}

		private CircuitNode SelectPending(HashSet<CircuitNode> pending)
		{
			for (int i = nodes.Count - 1; i >= 0; i--)
				if (pending.Contains(nodes[i]))
					return nodes[i];
			Debug.Assert(false);
			return null;
		}

		private void Evaluate1(HashSet<CircuitNode> pending, HashSet<CircuitNode> evaluated)
		{
			pending.Clear();
			int evaluatedCount;
			do
			{
				evaluatedCount = evaluated.Count;
				foreach (CircuitNode node in nodes)
					Evaluate2(node, pending, evaluated);
			} while (evaluated.Count != evaluatedCount);
		}

		private bool Evaluate2(CircuitNode node, HashSet<CircuitNode> pending, HashSet<CircuitNode> evaluated)
		{
			if (evaluated.Contains(node))
				return true;
			if (pending.Contains(node))
				return false;
			pending.Add(node);
			bool blocked = false;
			foreach (CircuitNode dependency in node.DependsOn())
			{
				if (evaluated.Contains(dependency))
					continue;
				if (!Evaluate2(dependency, pending, evaluated))
					blocked = true;
			}
			if (blocked)
			{
				return false;
			}
			else
			{
				node.Evaluate();
				pending.Remove(node);
				evaluated.Add(node);
				return true;
			}
		}
	}
}
