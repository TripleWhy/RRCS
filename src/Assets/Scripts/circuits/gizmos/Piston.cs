using UnityEngine;

namespace AssemblyCSharp.gizmos
{
    public class Piston : Gizmo
    {
        private float currentPosition = 0; // m
        private float currentVelocity = 0; // cm/s

        private int targetVelocity = 0; // cm/s
        private float currentMaxAcceleration = 0; // cm/(s^2)

        public Piston(CircuitManager manager) : base(manager, 3)
        {
        }

        protected override void EvaluateOutputs()
        {
            if (InValue(0) != 0)
            {
                int accelerationTime = getAccelerationTime();
                float targetPosition = InValue(2) / 100f; // m

                if (InValue(1) == 0 && isMoveToTarget())
                {
                    // Instant
                    currentPosition = Mathf.Clamp(targetPosition, 0, getMaxTravelDistance());
                    currentVelocity = 0;
                }
                else
                {
                    if (accelerationTime == 0)
                    {
                        currentVelocity = InValue(1);
                    }
                    else
                    {
                        if (targetVelocity != InValue(1))
                        {
                            targetVelocity = InValue(1);
                            currentMaxAcceleration =
                                Mathf.Abs(targetVelocity - currentVelocity) * (1f / accelerationTime);
                        }

                        currentVelocity += Mathf.Clamp((targetVelocity - currentVelocity),
                            -currentMaxAcceleration, currentMaxAcceleration);
                    }

                    if (isMoveToTarget())
                    {
                        float clampPositionMin; // m
                        float clampPositionMax; // m

                        float velocityTowardsGoal = currentVelocity;

                        if (Mathf.Sign(targetPosition - currentPosition) != Mathf.Sign(currentVelocity))
                        {
                            velocityTowardsGoal = -velocityTowardsGoal;
                        }

                        if (velocityTowardsGoal < 0)
                        {
                            clampPositionMin = targetPosition;
                            clampPositionMax = currentPosition;
                        }
                        else
                        {
                            clampPositionMax = targetPosition;
                            clampPositionMin = currentPosition;
                        }


                        currentPosition = Mathf.Clamp(currentPosition + (velocityTowardsGoal * 0.001f), clampPositionMin,
                            clampPositionMax);

                        if (currentPosition == targetPosition)
                        {
                            currentVelocity = 0;
                        }
                    }
                    else
                    {
                        currentPosition += currentVelocity * 0.001f;
                    }

                    float clamped = Mathf.Clamp(currentPosition, 0, getMaxTravelDistance());
                    if (currentPosition != clamped)
                    {
                        currentVelocity = 0;
                    }

                    currentPosition = clamped;
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
                NodeSetting.CreateSetting(NodeSetting.SettingType.MoveToTarget),
                NodeSetting.CreateSetting(NodeSetting.SettingType.MaxTravelDistance),
            };
        }

        public override string getGizmoValueString()
        {
            if (getAccelerationTime() > 0)
            {
                return string.Format("{0:0.00}m/{1:0.0}m @{2:0.0}cm/s", currentPosition, getMaxTravelDistance(),
                    currentVelocity);
            }
            else
            {
                return string.Format("{0:0.00}m/{1:0.0}m", currentPosition, getMaxTravelDistance());
            }
        }

        private int getAccelerationTime()
        {
            int val = (int) settings[0].currentValue;
            return val < 0 ? 0 : val;
        }

        private float getMaxTravelDistance()
        {
            int val = (int) settings[2].currentValue;
            return val < 0 ? 0 : val / 10f;
        }

        public bool isMoveToTarget()
        {
            return (bool) settings[1].currentValue;
        }
    }
}