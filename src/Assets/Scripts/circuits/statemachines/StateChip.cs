﻿namespace AssemblyCSharp
{
	using System.Collections.Generic;

	public class StateChip : Chip
	{
		private bool isActive = false;
		public bool prevActive = false;

		public delegate void StateNameChangedEventHandler(StateChip source, string stateName);
		public event StateNameChangedEventHandler StateNameChanged = delegate { };
		public delegate void StateActiveChangedEventHandler(StateChip source, bool stateActive);
		public event StateActiveChangedEventHandler StateActiveChanged = delegate { };

		public StateChip(CircuitManager manager)
			: base(manager, 0, 3, false, Port.PortType.StatePort)
		{
			EmitEvaluationRequired();
		}

		public override int IconIndex
		{
			get
			{
				return 32;
			}
		}

		public override void Evaluate()
		{
			EvaluateOutputs();

			prevActive = isActive;
		}

		protected override void EvaluateOutputs()
		{
			outputPorts[0].Value = outputPorts[1].Value = outputPorts[2].Value = 0;

			if (isActive)
			{
				outputPorts[1].Value = 1;
				if (!prevActive)
					outputPorts[0].Value = 1;
			}
			else if (prevActive)
			{
				outputPorts[2].Value = 1;
			}
		}

		override protected NodeSetting[] CreateSettings()
		{
			return new NodeSetting[]
			{
				NodeSetting.CreateSetting(NodeSetting.SettingType.StateMinTimeInState),
				NodeSetting.CreateSetting(NodeSetting.SettingType.StateValue0),
				NodeSetting.CreateSetting(NodeSetting.SettingType.StateValue1),
				NodeSetting.CreateSetting(NodeSetting.SettingType.StateValue2),
				NodeSetting.CreateSetting(NodeSetting.SettingType.StateName),
			};
		}

		public int MinTimeInState
		{
			get
			{
				return (int)settings[0].currentValue;
			}
		}

		public int Value0
		{
			get
			{
				return (int)settings[1].currentValue;
			}
		}

		public int Value1
		{
			get
			{
				return (int)settings[2].currentValue;
			}
		}

		public int Value2
		{
			get
			{
				return (int)settings[3].currentValue;
			}
		}

		public string StateName
		{
			get
			{
				return (string)settings[4].currentValue;
			}
		}

		public override void SetSetting(NodeSetting setting, object value)
		{
			if (setting.type != NodeSetting.SettingType.StateName)
			{
				base.SetSetting(setting, value);
			}
			else
			{
				if (value == setting.currentValue)
					return;
				setting.currentValue = value;
				StateNameChanged(this, (string)value);
			}
		}


		public bool Active
		{
			get
			{
				return isActive;
			}
			set
			{
				if (value == isActive)
					return;
				isActive = value;
				StateActiveChanged(this, value);
				EmitEvaluationRequired();
			}
		}

		public override IEnumerable<CircuitNode> SimpleDependsOn()
		{
			StatePort rootPort = statePort.FindConnectedRootPort();
			if (rootPort != null)
				yield return rootPort.node;
		}

		public override IEnumerable<CircuitNode> SimpleDependingOnThis()
		{
			//TODO
			throw new System.NotImplementedException();
		}
	}
}
