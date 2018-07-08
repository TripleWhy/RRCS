namespace AssemblyCSharp
{
	using System;

	public class RandomChip : Chip
	{
		private static Random random = new Random();
		private bool clearValue = false;

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
			if (InBool(0))
			{
				try
				{
					outputPorts[0].Value = random.Next(InValue(1), InValue(2));
					clearValue = !(bool)settings[0].currentValue;
					EmitEvaluationRequired();
				}
				catch (ArgumentOutOfRangeException)
				{
					outputPorts[0].Value = 0;
				}
			}
			else
			{
				if (clearValue)
					outputPorts[0].Value = 0;
			}
		}

		override protected NodeSetting[] CreateSettings()
		{
			return new NodeSetting[]
			{
				NodeSetting.CreateSetting(NodeSetting.SettingType.ContinuousOutput),
			};
		}
	}
}
