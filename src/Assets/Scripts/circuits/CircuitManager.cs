namespace AssemblyCSharp
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	public class CircuitManager
	{
		private readonly List<CircuitNode> nodes = new List<CircuitNode>();
		private readonly List<CircuitNode> cachedEvaluationOrder = new List<CircuitNode>();
		private bool graphChanged = true;
		private bool evaluationRequired = true;
		public int CurrentTick { get; private set; }

		public void AddNode(CircuitNode node)
		{
			Debug.Assert(!nodes.Contains(node));
			if (nodes.Contains(node))
				return;

			node.RingEvaluationPriority = nodes.Count;
			nodes.Add(node);

			node.ConnectionChanged += Node_ConnectionChanged;
			node.EvaluationRequired += Node_EvaluationRequired;
			InvalidateOrder();
		}

		public void RemoveNode(CircuitNode node)
		{
			Debug.Assert(object.ReferenceEquals(nodes[node.RingEvaluationPriority], node));
			nodes.RemoveAt(node.RingEvaluationPriority);
			for (int i = node.RingEvaluationPriority, s = nodes.Count; i < s; i++)
			{
				Debug.Assert(nodes[i].RingEvaluationPriority == i + 1);
				nodes[i].RingEvaluationPriority = i;
			}

			node.EvaluationRequired -= Node_EvaluationRequired;
			node.RingEvaluationPriority = -1;
			InvalidateOrder();
		}

		public List<CircuitNode> Nodes
		{
			get
			{
				return nodes;
			}
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
			InvalidateOrder();
		}

		private void InvalidateOrder()
		{
			graphChanged = true;
		}

		private void Invalidate()
		{
			evaluationRequired = true;
		}

		internal void Clear()
		{
			for (int i = nodes.Count - 1; i >= 0; --i)
				RemoveNode(nodes[i]);
			Debug.Assert(nodes.Count == 0);
		}

		private void Node_ConnectionChanged(CircuitNode source)
		{
			InvalidateOrder();
		}

		private void Node_EvaluationRequired(CircuitNode source)
		{
			Invalidate();
		}

		public void Tick()
		{
			CurrentTick++;
			foreach (CircuitNode node in nodes)
				node.Tick();
			EvaluateIfNecessary();
		}

		private void EvaluateIfNecessary()
		{
			float t0 = Time.realtimeSinceStartup;

			if (graphChanged)
			{
				graphChanged = false;
				EvaluateOrder();
				Debug.Assert(graphChanged == false);
				evaluationRequired = true;
			}
			if (evaluationRequired)
			{
				evaluationRequired = false; //Clearing the flag before Evaluate() allows Evaluate() to set the flag again.
				Evaluate();
			}
			else
				return;

			float t1 = Time.realtimeSinceStartup;
			Debug.Log("Time: " + (t1 - t0));
		}

		public void EvaluateOrder()
		{
			//TODO this whole evaluation stuff seems overly complicated. Find a better solution?

			cachedEvaluationOrder.Clear();
			HashSet<CircuitNode> pending = new HashSet<CircuitNode>();
			HashSet<CircuitNode> evaluated = new HashSet<CircuitNode>();
			List<CircuitNode> selected = new List<CircuitNode>();

			int loopRuns = 0;
			for (; loopRuns < 10000; loopRuns++)
			{
				EvaluateOrder1(pending, evaluated, cachedEvaluationOrder);
				if (pending.Count == 0)
					break;
				while (true)
				{
					CircuitNode n = SelectPending(pending);
					selected.Add(n);
					evaluated.Add(n);
					int evaluatedCount = evaluated.Count;
					EvaluateOrder1(pending, evaluated, cachedEvaluationOrder);
					if (evaluated.Count > evaluatedCount)
						break;
				}
				for (int i = selected.Count - 1; i >= 0; --i)
					evaluated.Remove(selected[i]);
				selected.Clear();
			}

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

		private void EvaluateOrder1(HashSet<CircuitNode> pending, HashSet<CircuitNode> evaluated, List<CircuitNode> nodeOrder)
		{
			pending.Clear();
			int evaluatedCount;
			do
			{
				evaluatedCount = evaluated.Count;
				foreach (CircuitNode node in nodes)
					EvaluateOrder2(node, pending, evaluated, nodeOrder);
			} while (evaluated.Count != evaluatedCount);
		}

		private bool EvaluateOrder2(CircuitNode node, HashSet<CircuitNode> pending, HashSet<CircuitNode> evaluated, List<CircuitNode> nodeOrder)
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
				if (!EvaluateOrder2(dependency, pending, evaluated, nodeOrder))
					blocked = true;
			}
			if (blocked)
			{
				return false;
			}
			else
			{
				nodeOrder.Add(node);
				pending.Remove(node);
				evaluated.Add(node);
				return true;
			}
		}

		private void Evaluate()
		{
			Debug.Assert(cachedEvaluationOrder.Count == nodes.Count);
			foreach (CircuitNode node in cachedEvaluationOrder)
				node.Evaluate();
		}
	}
}
