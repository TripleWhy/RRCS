namespace AssemblyCSharp
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	public class CircuitManager
	{
		private List<CircuitNode> nodes = new List<CircuitNode>();
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

		private void ReplaceNodePriorities(List<CircuitNode> newNodes)
		{
			Debug.Assert(newNodes.Count == nodes.Count);
			nodes = newNodes;
			for (int i = 0; i < nodes.Count; i++)
				nodes[i].RingEvaluationPriority = i;
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

			List<CircuitNode> newEvaluationOrder = new List<CircuitNode>(nodes.Count);
			HashSet<CircuitNode> pending = new HashSet<CircuitNode>();
			HashSet<CircuitNode> evaluated = new HashSet<CircuitNode>();
			HashSet<CircuitNode> visited = new HashSet<CircuitNode>();

			while (true)
			{
				EvaluateOrder1(pending, evaluated, newEvaluationOrder);
				if (pending.Count == 0)
					break;

				CircuitNode n = SelectPending(pending, visited);
				if (n == null)
					return;
				Debug.Assert(pending.Contains(n));
				Debug.Assert(!evaluated.Contains(n));
				evaluated.Add(n);
				newEvaluationOrder.Add(n);
			}
			ReplaceNodePriorities(newEvaluationOrder);
		}

		private CircuitNode SelectPending(HashSet<CircuitNode> pending, HashSet<CircuitNode> visited)
		{
			visited.Clear();
			for (int i = nodes.Count - 1; i >= 0; i--)
			{
				CircuitNode node = nodes[i];
				if (!pending.Contains(node))
					continue;

				CircuitNode n = Visit(node, pending, visited);
				if (n != null)
					return n;
			}
			Debug.Assert(false);
			return null;
		}

		private CircuitNode Visit(CircuitNode node, HashSet<CircuitNode> pending, HashSet<CircuitNode> visited)
		{
			visited.Add(node);
			foreach (CircuitNode dependency in node.DependsOn())
			{
				if (!pending.Contains(dependency))
					continue;
				if (visited.Contains(dependency))
					return node;
				CircuitNode n = Visit(dependency, pending, visited);
				if (n != null)
					return n;
			}
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
			foreach (CircuitNode node in nodes)
				node.Evaluate();
		}
	}
}
