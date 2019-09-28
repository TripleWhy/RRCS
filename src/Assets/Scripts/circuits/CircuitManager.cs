namespace AssemblyCSharp
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class CircuitManager
	{
		private const bool dbgForceReEvaluation = false;

		private List<CircuitNode> nodes = new List<CircuitNode>();
		private bool graphChanged = true;
		private bool evaluationRequired = true;
		public int CurrentTick { get; private set; }
		public bool UseIntValuesOnly { get; set; }

		public delegate void VoidEventHandler();
		public event VoidEventHandler SimulationPauseRequested = delegate { };

		internal void RequestSimulationPause()
		{
			SimulationPauseRequested();
		}

		public void AddNode(CircuitNode node)
		{
			DebugUtils.Assert(!nodes.Contains(node));
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
			DebugUtils.Assert(object.ReferenceEquals(nodes[node.RingEvaluationPriority], node));
			nodes.RemoveAt(node.RingEvaluationPriority);
			for (int i = node.RingEvaluationPriority, s = nodes.Count; i < s; i++)
			{
				DebugUtils.Assert(nodes[i].RingEvaluationPriority == i + 1);
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
			DebugUtils.Assert(object.Equals(nodes[oldPriority], node));
			if (newPriority == oldPriority)
				return;
			nodes.RemoveAt(oldPriority);
			nodes.Insert(newPriority, node);

			for (int i = Math.Min(oldPriority, newPriority), max = Math.Max(oldPriority, newPriority); i <= max; i++)
				nodes[i].RingEvaluationPriority = i;

			DebugUtils.Assert(object.Equals(nodes[newPriority], node));
			DebugUtils.Assert(node.RingEvaluationPriority == newPriority);
			InvalidateOrder();
		}

		private void ReplaceNodePriorities(List<CircuitNode> newNodes)
		{
			DebugUtils.Assert(newNodes.Count == nodes.Count);
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
			DebugUtils.Assert(nodes.Count == 0);
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
			float t0 = UnityEngine.Time.realtimeSinceStartup;

			if (graphChanged || dbgForceReEvaluation)
			{
				graphChanged = false;
				EvaluateOrder();
				DebugUtils.Assert(graphChanged == false);
				evaluationRequired = true;
			}
			if (evaluationRequired)
			{
				evaluationRequired = false; //Clearing the flag before Evaluate() allows Evaluate() to set the flag again.
				Evaluate();
			}
			else
				return;

			float t1 = UnityEngine.Time.realtimeSinceStartup;
			UnityEngine.Debug.Log("Time: " + (t1 - t0));
		}

		public void EvaluateOrder()
		{
			ReplaceNodePriorities(OrderDfsR());
		}

		private class EdgeComparer : IComparer<Connection>
		{
			public int Compare(Connection x, Connection y)
			{
				int xTargetPrio = x.TargetPort.node.RingEvaluationPriority;
				int yTargetPrio = y.TargetPort.node.RingEvaluationPriority;
				if (xTargetPrio != yTargetPrio)
					return xTargetPrio.CompareTo(yTargetPrio);
				int xSourcePrio = x.SourcePort.node.RingEvaluationPriority;
				int ySourcePrio = y.SourcePort.node.RingEvaluationPriority;
				return ySourcePrio.CompareTo(xSourcePrio);
			}
		}
		private static readonly EdgeComparer edgeComparer = new EdgeComparer();
		private enum DfsMark
		{
			Unmarked,
			Temmporary,
			Permanent,
		}
		//Based on https://en.wikipedia.org/wiki/Topological_sorting#Depth-first_search, but with reversed logic and loop resolution.
		private List<CircuitNode> OrderDfsR()
		{
			Dictionary<CircuitNode, DfsMark> marks = new Dictionary<CircuitNode, DfsMark>(); //A simple array should work too
			foreach (CircuitNode n in nodes)
				marks.Add(n, DfsMark.Unmarked);

			List<CircuitNode> L = new List<CircuitNode>();
			Stack<CircuitNode> path = new Stack<CircuitNode>();
			HashSet<Connection> removedEdges = new HashSet<Connection>();
			foreach (CircuitNode n in nodes)
			{
				if (marks[n] != DfsMark.Unmarked)
					continue;
				DebugUtils.Assert(ReferenceEquals(n, nodes.First(cn => marks[cn] == DfsMark.Unmarked)));
				OrderDfsRVisit0(n, marks, L, path, removedEdges);
				DebugUtils.Assert(marks[n] == DfsMark.Permanent);
			}
			DebugUtils.Assert(!nodes.Any((CircuitNode n) => marks[n] != DfsMark.Permanent));

			return L;
		}
		private CircuitNode OrderDfsRVisit0(CircuitNode n, Dictionary<CircuitNode, DfsMark> marks, List<CircuitNode> L, Stack<CircuitNode> path, HashSet<Connection> removedEdges)
		{
			path.Push(n);
			CircuitNode result = OrderDfsRVisit1(n, marks, L, path, removedEdges);
			CircuitNode popped = path.Pop();
			DebugUtils.Assert(ReferenceEquals(popped, n));
			return result;
		}

		private CircuitNode OrderDfsRVisit1(CircuitNode n, Dictionary<CircuitNode, DfsMark> marks, List<CircuitNode> L, Stack<CircuitNode> path, HashSet<Connection> removedEdges)
		{
			switch (marks[n])
			{
				case DfsMark.Unmarked:
					break;
				case DfsMark.Permanent:
					return null;
				case DfsMark.Temmporary:
					throw new InvalidOperationException();
			}

			marks[n] = DfsMark.Temmporary;
			while (true)
			{
				CircuitNode backtrackTo = null;
				foreach (Connection e in n.IncomingConnections().Where(a => !removedEdges.Contains(a)).OrderBy((Connection c) => c.SourcePort.node))
				{
					DebugUtils.Assert(ReferenceEquals(e.TargetPort.node, n));
					CircuitNode m = e.SourcePort.node;
					if (marks[m] == DfsMark.Temmporary)
					{
						SortedSet<CircuitNode> loop = new SortedSet<CircuitNode>();
						foreach (CircuitNode cn in path)
						{
							loop.Add(cn);
							if (ReferenceEquals(cn, m))
								break;
						}
						DebugUtils.Assert(loop.Count > 1);
						foreach (CircuitNode cn in loop)
						{
							foreach (Connection c in cn.IncomingConnections().Where(a => !removedEdges.Contains(a)).OrderBy(a => a, edgeComparer))
							{
								DebugUtils.Assert(ReferenceEquals(c.TargetPort.node, cn));
								CircuitNode src = c.SourcePort.node;
								if (!loop.Contains(src))
									continue;
								removedEdges.Add(c);
								backtrackTo = cn;
								goto afterLoop;
							}
						}
						throw new InvalidOperationException();
					}

					backtrackTo = OrderDfsRVisit0(m, marks, L, path, removedEdges);
					if (backtrackTo != null)
						break;
				}
				afterLoop:
				if (backtrackTo == null)
					break;
				else if (!ReferenceEquals(backtrackTo, n))
				{
					marks[n] = DfsMark.Unmarked;
					return backtrackTo;
				}
			}

			if (marks[n] == DfsMark.Temmporary)
			{
				marks[n] = DfsMark.Permanent;
				L.Add(n);
			}
			else
				throw new InvalidOperationException();
			return null;
		}

		private void Evaluate()
		{
			foreach (CircuitNode node in nodes)
				node.Evaluate();
		}
	}
}
