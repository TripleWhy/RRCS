using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp
{
    public class StateMachineChip : Chip
    {
        private int timeInState = 0;

        private StateChip activeState;
        private StateChip prevActiveState;

        public StateMachineChip(CircuitManager manager) : base(manager, 1, 5, true, StatePort.StatePortType.Root)
        {
            EmitEvaluationRequired();
        }

        override public int IconIndex
        {
            get { return -1; }
        }

        private void resetActiveState()
        {
            activeState = statePort.getNextState();
            if (activeState != null)
                activeState.Active = true;
            timeInState = 0;
        }

        private bool isActive()
        {
            return !IsResetSet && !inputPorts[0].IsConnected || inputPorts[0].GetValue() != 0;
        }

        public override void Evaluate()
        {
            Debug.Assert(statePort != null);

            
            if (IsResetSet)
            {
                for (int i = 0; i < outputPortCount; ++i)
                    outputPorts[i].Value = 0;
            }
            else if (isActive())
            {
                prevActiveState = activeState;
                activeState = statePort.getConnectedActiveState();

                timeInState++;

                if (activeState == null)
                    resetActiveState();

                if (activeState != null && timeInState > 0 &&
                    timeInState >= (int) activeState.settings[0].currentValue)
                {
                    var nextState = activeState.statePort.getNextStateAfterValidTransition();
                    if (nextState != null)
                    {
                        activeState.Active = false;
                        nextState.Active = true;
                        activeState = nextState;
                        timeInState = 0;
                    }
                }

                EvaluateOutputs();

                EmitEvaluationRequired();
            }
            outputPorts[outputPortCount].Value = ResetValue;
        }

        override protected void EvaluateOutputs()
        {
            if (activeState != null)
            {
                outputPorts[0].Value = (int) activeState.settings[1].currentValue;
                outputPorts[1].Value = (int) activeState.settings[2].currentValue;
                outputPorts[2].Value = (int) activeState.settings[3].currentValue;
                outputPorts[3].Value = timeInState * 100;
            }
            else
            {
                outputPorts[0].Value = 0;
                outputPorts[1].Value = 0;
                outputPorts[2].Value = 0;
                outputPorts[3].Value = 0;
            }

            outputPorts[4].Value = prevActiveState != activeState ? 1 : 0;
        }

        public override IEnumerable<CircuitNode> DependsOn()
        {
            var transitionPorts = statePort.getAllTransitionEnabledPorts();
            foreach (var port in transitionPorts)
            {
                if (port.IsConnected && !ReferenceEquals(port.connections[0].sourcePort.node, this))
                    yield return port.connections[0].sourcePort.node;
            }
        }
    }
}