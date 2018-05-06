namespace AssemblyCSharp
{
	using System;

	public class RandomChip : Chip
	{
		private static Random random = new Random();

		public RandomChip(CircuitManager manager) : base(manager, 3, 1, false)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 27;
			}
		}

		override protected void EvaluateOutputs()
		{
			if (ToBool(inputPorts[0]))
			{
				try
				{
					outputPorts[0].Value = random.Next(ToInt(inputPorts[1]), ToInt(inputPorts[2]));
				}
				catch (ArgumentOutOfRangeException)
				{
				}
			}
		}
	}
}
