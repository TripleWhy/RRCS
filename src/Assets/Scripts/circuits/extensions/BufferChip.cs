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

		public override void Evaluate()
		{
			if (IsResetSet)
			{
				queue.Clear();
				outputPorts[0].Value = null;
				outputPorts[outputPortCount].Value = ResetValue;
			}
			else
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
				outputPorts[1].Value = queue.Count;
				outputPorts[outputPortCount].Value = false;
			}
		}

		protected override void EvaluateOutputs()
		{
		}
	}
}
