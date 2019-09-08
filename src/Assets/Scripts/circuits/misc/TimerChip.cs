namespace AssemblyCSharp
{
	public class TimerChip : Chip
	{
		private int remainingTicks = -1;
		private int remainingSeconds = 0;
		private int duration = 0;

		public TimerChip(CircuitManager manager) : base(manager, 2, 2, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 21;
			}
		}

		private void Start()
		{
			remainingTicks = InInt(1);
			remainingSeconds = (remainingTicks + 9) / 10;
		}

		private bool IsOn
		{
			get
			{
				return InBool(0) && !IsResetSet;
			}
		}

		public override void Tick()
		{
			if (!IsOn) //Reads the input from last tick, but it probably doesn't matter.
				return;
			remainingTicks--;
			if (remainingTicks < 0)
				return;
			int secs = (remainingTicks + 9) / 10;
			if (secs != remainingSeconds || remainingTicks == 0)
			{
				EmitEvaluationRequired();
				remainingSeconds = secs;
			}
		}

		override public void Evaluate()
		{
			outputPorts[outputPortCount].Value = ResetValue;
			int dur = InInt(1);
			if (IsResetSet || dur != duration)
			{
				duration = dur;
				Start();
			}

			if (remainingTicks == 0 && IsOn)
			{
				outputPorts[0].Value = true;
				if ((bool)settings[0].currentValue)
					Start();
				EmitEvaluationRequired();
			}
			else
				outputPorts[0].Value = false;
			outputPorts[1].Value = remainingSeconds;
		}

		override protected void EvaluateOutputs()
		{
		}

		override protected NodeSetting[] CreateSettings()
		{
			return new NodeSetting[]
			{
				NodeSetting.CreateSetting(NodeSetting.SettingType.LoopingTimer),
			};
		}
	}
}
