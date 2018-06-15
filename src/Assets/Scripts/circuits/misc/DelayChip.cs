namespace AssemblyCSharp
{
	using Priority_Queue;
	using System;
	using System.Collections.Generic;

	public class DelayChip : Chip
	{
		public class QueueEntry : FastPriorityQueueNode
		{
			public readonly int outputTick;
			public readonly int value;

			public QueueEntry(int outputTick, int value)
			{
				this.outputTick = outputTick;
				this.value = value;
			}
		}

		private int lastInput = 0;
		private FastPriorityQueue<QueueEntry> queue = new FastPriorityQueue<QueueEntry>(20);

		public DelayChip(CircuitManager manager) : base(manager, 2, 1, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 22;
			}
		}

		private bool ShouldOutput()
		{
			return queue.Count != 0 && Manager.CurrentTick >= queue.First.outputTick && !ToBool(inputPorts[2]);
		}

		public override void Tick()
		{
			if (ShouldOutput())
				EmitEvaluationRequired();
		}

		override public void Evaluate()
		{
			if (ToBool(inputPorts[2]))
				queue.Clear();
			else
				EvaluateOutputs();
		}

		override protected void EvaluateOutputs()
		{
			if (ShouldOutput())
			{
				outputPorts[0].Value = queue.Dequeue().value;
				EmitEvaluationRequired();
			}
			else
				outputPorts[0].Value = 0;

			int value = ToInt(inputPorts[0]);
			if (value == lastInput)
				return;
			lastInput = value;
			if (!ToBool(value))
				return;
			int delay = Math.Max(1, ToInt(inputPorts[1]));
			int outputTick = Manager.CurrentTick + delay;
			QueueEntry entry = new QueueEntry(outputTick, value);
			if (queue.Count >= queue.MaxSize)
				queue.Dequeue();
			queue.Enqueue(entry, outputTick);
		}
	}
}
