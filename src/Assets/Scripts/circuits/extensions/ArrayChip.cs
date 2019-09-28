namespace AssemblyCSharp
{
	using System;
	using System.Collections.Generic;

	public class ArrayChip : Chip
	{
		const int maxSize = 2048;
		List<IConvertible> data = new List<IConvertible>(maxSize);

		public ArrayChip(CircuitManager manager)
			: base(manager, 5, 4, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 44;
			}
		}

		private int DataIndex(int index)
		{
			index = Math.Min(index, data.Count);
			if (index < 0)
				return Math.Max(0, data.Count + index);
			return index;
		}

		protected override void Reset()
		{
			data.Clear();
			base.Reset();
		}

		protected override void EvaluateOutputs()
		{
			if (InBool(2))
			{
				IConvertible insertValue = InValue(0);
				int insertIndex = DataIndex(InInt(1));
				if (data.Count < maxSize)
					data.Insert(insertIndex, insertValue);
				EmitEvaluationRequired();
			}
			if (data.Count > 0)
			{
				int removeIndex = Math.Min(data.Count - 1, DataIndex(InInt(3)));
				outputPorts[0].Value = data[removeIndex];
				if (InBool(4))
				{
					data.RemoveAt(removeIndex);
					EmitEvaluationRequired();
				}
			}
			else
				outputPorts[0].Value = null;
			if (data.Count > 0)
			{
				outputPorts[1].Value = data[0];
				outputPorts[2].Value = data[data.Count - 1];
			}
			else
			{
				outputPorts[1].Value = null;
				outputPorts[2].Value = null;
			}
			outputPorts[3].Value = data.Count;
			outputPorts[outputPortCount].Value = false;
		}
	}
}
