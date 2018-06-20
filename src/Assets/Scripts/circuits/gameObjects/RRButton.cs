namespace AssemblyCSharp
{
	public class RRButton : CircuitNode
	{
		private bool lastPressed = false;
		private bool pressed = false;
		private bool released = false;

		public RRButton(CircuitManager manager) : base(manager, 0, 3, false)
		{
		}

		protected override NodeSetting[] CreateSettings()
		{
			return new NodeSetting[] { NodeSetting.CreateSetting(NodeSetting.SettingType.SendPlayerId) };
		}

		protected override void EvaluateOutputs()
		{
			if (pressed && released && lastPressed)
				pressed = released = false;
			if (pressed)
			{
				outputPorts[2].Value = 0;
				if (lastPressed)
					outputPorts[0].Value = 0;
				else
				{
					outputPorts[0].Value = outputPorts[1].Value = GetOutputValue();
					EmitEvaluationRequired();
				}
			}
			else
			{
				outputPorts[0].Value = outputPorts[1].Value = 0;
				if (lastPressed)
				{
					outputPorts[2].Value = GetOutputValue();
					EmitEvaluationRequired();
				}
				else
					outputPorts[2].Value = 0;
			}
			lastPressed = pressed;
		}

		private int GetOutputValue()
		{
			if ((bool)settings[0].currentValue)
				return RRCSManager.Instance.CurrentPlayerId;
			else
				return 1;
		}

		public void Press()
		{
			pressed = true;
			EmitEvaluationRequired();
		}

		public void Release()
		{
			released = true;
			EmitEvaluationRequired();
		}
	}
}
