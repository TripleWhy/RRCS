namespace AssemblyCSharp
{
	using System;
	using System.Collections.Generic;

	public class StateChip : Chip
	{
		private bool isActive = false;
		private bool lastWasActive = false;
		private StateMachineChip stateMachine;

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

		protected override Type ExpectedOutputType(int outputIndex)
		{
			return typeof(bool);
		}

		public override void Evaluate()
		{
			EvaluateOutputs();
		}

		protected override void EvaluateOutputs()
		{
			outputPorts[0].Value = outputPorts[1].Value = outputPorts[2].Value = false;
			if (isActive)
			{
				outputPorts[1].Value = true;
				if (!lastWasActive)
					outputPorts[0].Value = true;
			}
			else if (lastWasActive)
			{
				outputPorts[2].Value = true;
			}
			lastWasActive = isActive;
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

		internal StateMachineChip StateMachineUnchecked
		{
			get
			{
				return stateMachine;
			}
		}

		public StateMachineChip StateMachine
		{
			get
			{
				CheckStateMachine();
				return stateMachine;
			}
			set
			{
				stateMachine = value;
				if (stateMachine == null)
					Active = false;
			}
		}

		internal void CheckStateMachine()
		{
			DebugUtils.Assert(object.ReferenceEquals(stateMachine, FindConnectedRoot()));
		}

		public override IEnumerable<Connection> IncomingConnections()
		{
			foreach (StateMachineTransition transition in statePort.connections)
			{
				if (!object.ReferenceEquals(transition.TargetStatePort.node, this))
					continue;
				if (transition.TransitionEnabledPort != null)
					foreach (DataConnection transitionEnabledConnection in transition.TransitionEnabledPort.connections)
						yield return transitionEnabledConnection;
				yield return transition;
			}
		}

		public override IEnumerable<Connection> OutgoingConnections()
		{
			throw new System.NotImplementedException();
		}

		private StateMachineChip FindConnectedRoot()
		{
			return FindConnectedRoot(statePort, new HashSet<StatePort>());
		}

		private StateMachineChip FindConnectedRoot(StatePort port, HashSet<StatePort> visited)
		{
			if (port.IsStateRootPort)
				return (StateMachineChip)port.node;
			visited.Add(port);

			foreach (StateMachineTransition connection in port.connections)
			{
				StatePort otherPort = connection.GetOtherPort(port);
				if (otherPort != null && otherPort.IsStatePort && !visited.Contains(otherPort))
				{
					StateMachineChip root = FindConnectedRoot(otherPort, visited);
					if (root != null)
						return root;
				}
			}
			return null;
		}
	}
}
