using UnityEngine;

namespace AssemblyCSharp.gizmos
{
    public class Rotator : Gizmo
    {
        private float currentPosition = 0; // deg
        private float currentVelocity = 0; // deg/s

        private int targetVelocity = 0; // rpm
        private float currentMaxAcceleration = 0; // deg/(s^2)

        public Rotator(CircuitManager manager) : base(manager, 3)
        {
        }

        protected override void EvaluateOutputs()
        {
            if (InValue(0) != 0)
            {
                int accelerationTime = getAccelerationTime();

                if (InValue(1) == 0 && isMoveToTarget())
                {
                    // Instant
                    currentPosition = Mathf.Repeat(InValue(2), 360);
                    currentVelocity = 0;
                }
                else
                {
                    if (accelerationTime == 0)
                    {
                        // 1 RPM = 6 deg/s
                        currentVelocity = InValue(1) * 6f;
                    }
                    else
                    {
                        if (targetVelocity != InValue(1))
                        {
                            targetVelocity = InValue(1);
                            currentMaxAcceleration =
                                Mathf.Abs(targetVelocity * 6f - currentVelocity) * (1f / accelerationTime);
                        }

                        currentVelocity += Mathf.Clamp((targetVelocity * 6f - currentVelocity),
                            -currentMaxAcceleration, currentMaxAcceleration);
                    }

                    if (isMoveToTarget())
                    {
                        float clampPositionMin;
                        float clampPositionMax;
                        float targetPosition = Mathf.Repeat(InValue(2), 360);

                        if (currentVelocity < 0)
                        {
                            if (targetPosition <= currentPosition)
                            {
                                clampPositionMin = targetPosition;
                            }
                            else
                            {
                                clampPositionMin = targetPosition - 360f;
                            }

                            clampPositionMax = currentPosition;
                        }
                        else
                        {
                            if (targetPosition >= currentPosition)
                            {
                                clampPositionMax = targetPosition;
                            }
                            else
                            {
                                clampPositionMax = targetPosition + 360f;
                            }

                            clampPositionMin = currentPosition;
                        }


                        currentPosition = Mathf.Clamp(currentPosition + (currentVelocity * 0.1f), clampPositionMin,
                            clampPositionMax);

                        if (currentPosition == targetPosition)
                        {
                            currentVelocity = 0;
                        }
                    }
                    else
                    {
                        currentPosition += currentVelocity * 0.1f;
                    }

                    currentPosition = Mathf.Repeat(currentPosition, 360);
                }

                EmitEvaluationRequired();
            }
            else
            {
                currentVelocity = 0;
                targetVelocity = 0;
                currentMaxAcceleration = 0;
            }
        }

        override protected NodeSetting[] CreateSettings()
        {
            return new NodeSetting[]
            {
                NodeSetting.CreateSetting(NodeSetting.SettingType.AccelerationTime),
                NodeSetting.CreateSetting(NodeSetting.SettingType.MoveToTarget)
            };
        }

        public override string getGizmoValueString()
        {
            if (getAccelerationTime() > 0)
            {
                return string.Format("{0:0.0}° @{1:0.0}°/s", currentPosition, currentVelocity);
            }
            else
            {
                return string.Format("{0:0.0}°", currentPosition);
            }
            
        }

        private int getAccelerationTime()
        {
            int val = (int) settings[0].currentValue;
            return val < 0 ? 0 : val;
        }

        public bool isMoveToTarget()
        {
            return (bool) settings[1].currentValue;
        }
    }
}