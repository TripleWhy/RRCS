﻿namespace AssemblyCSharp
{
	using System;

	public class RRButton : CircuitNode
	{
		private bool lastPressed = false;
		private bool pressed = false;
		private bool released = false;

		public delegate void ButtonTextChangedEventHandler(RRButton source, string text);
		public event ButtonTextChangedEventHandler ButtonTextChanged = delegate { };

		public RRButton(CircuitManager manager) : base(manager, 0, 3, false)
		{
		}

		protected override Type ExpectedOutputType(int outputIndex)
		{
			return typeof(bool);
		}

		protected override NodeSetting[] CreateSettings()
		{
			NodeSetting textSetting = NodeSetting.CreateSetting(NodeSetting.SettingType.Text);
			textSetting.currentValue = "Press Me!";
			return new NodeSetting[]
			{
				NodeSetting.CreateSetting(NodeSetting.SettingType.SendPlayerId),
				textSetting,
			};
		}

		public override void SetSetting(NodeSetting setting, object value)
		{
			switch (setting.type)
			{
				case NodeSetting.SettingType.Text:
					if (value == setting.currentValue)
						return;
					setting.currentValue = value;
					ButtonTextChanged(this, (string)value);
					break;
				case NodeSetting.SettingType.SendPlayerId:
				{
					if (value == setting.currentValue)
						return;
					base.SetSetting(setting, value);
					Type acceptedType = (bool)value ? typeof(int) : typeof(bool);
					foreach (OutputPort output in outputPorts)
						output.expectedType = acceptedType;
					break;
				}
				default:
					base.SetSetting(setting, value);
					break;
			}
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

		private IConvertible GetOutputValue()
		{
			if ((bool)settings[0].currentValue)
				return RRCSManager.Instance.CurrentPlayerId;
			else
				return true;
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
