namespace AssemblyCSharp
{
	using System;
	using System.Collections.Generic;

	public class BufferChip : Chip
	{
		const int maxQueueSize = 2048;
		Queue<IConvertible> queue = new Queue<IConvertible>(maxQueueSize);

		public BufferChip(CircuitManager manager)
			: base(manager, 7, 2, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 42;
			}
		}

		protected override IConvertible DefaultOutputValue(int outputIndex)
		{
			if (outputIndex == 1)
				return 0;
			return base.DefaultOutputValue(outputIndex);
		}

		protected override void Reset()
		{
			queue.Clear();
			base.Reset();
		}

		protected override void EvaluateOutputs()
		{
			for (int i = 0; i < inputPortCount; i++)
			{
				if (!InBool(i))
					continue;
				if (queue.Count >= maxQueueSize)
					break;
				queue.Enqueue(InValue(i));
			}
			if (queue.Count == 0)
				outputPorts[0].Value = null;
			else
			{
				outputPorts[0].Value = queue.Dequeue();
				EmitEvaluationRequired();
			}
			outputPorts[outputPortCount].Value = false;
			outputPorts[1].Value = queue.Count;
		}
	}
}
