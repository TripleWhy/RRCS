using UnityEngine;

namespace AssemblyCSharp.gizmos
{
	public class Piston : Gizmo
	{
		private float currentPosition = 0; // m
		private float currentVelocity = 0; // cm/s

		private float targetVelocity = 0; // cm/s
		private float currentMaxAcceleration = 0; // cm/(s^2)

		public Piston(CircuitManager manager) : base(manager, 3)
		{
		}

		protected override void EvaluateOutputs()
		{
			if (InBool(0))
			{
				int accelerationTime = GetAccelerationTime();
				float inputVelocity = InFloat(1); // cm/s
				float targetPosition = InFloat(2) / 100f; // m

				if (inputVelocity == 0 && IsMoveToTarget())
				{
					// Instant
					currentPosition = Mathf.Clamp(targetPosition, 0, GetMaxTravelDistance());
					currentVelocity = 0;
					targetVelocity = 0;
				}
				else
				{
					if (accelerationTime == 0)
					{
						currentVelocity = inputVelocity;
					}
					else
					{
						if (targetVelocity != inputVelocity)
						{
							targetVelocity = inputVelocity;
							currentMaxAcceleration =
								Mathf.Abs(targetVelocity - currentVelocity) * (1f / accelerationTime);
						}

						currentVelocity += Mathf.Clamp((targetVelocity - currentVelocity),
							-currentMaxAcceleration, currentMaxAcceleration);
					}

					if (IsMoveToTarget())
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
							targetVelocity = 0;
						}
					}
					else
					{
						currentPosition += currentVelocity * 0.001f;
					}

					float clamped = Mathf.Clamp(currentPosition, 0, GetMaxTravelDistance());
					if (currentPosition != clamped)
					{
						currentVelocity = 0;
						targetVelocity = 0;
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

		public override string GetGizmoValueString()
		{
			if (GetAccelerationTime() > 0)
			{
				return string.Format("{0:0.00}m/{1:0.0}m @{2:0.0}cm/s", currentPosition, GetMaxTravelDistance(),
					currentVelocity);
			}
			else
			{
				return string.Format("{0:0.00}m/{1:0.0}m", currentPosition, GetMaxTravelDistance());
			}
		}

		private int GetAccelerationTime()
		{
			int val = (int) settings[0].currentValue;
			return val < 0 ? 0 : val;
		}

		private float GetMaxTravelDistance()
		{
			int val = (int) settings[2].currentValue;
			return val < 0 ? 0 : val / 10f;
		}

		public bool IsMoveToTarget()
		{
			return (bool) settings[1].currentValue;
		}

		public override void ResetGizmo()
		{
			currentPosition = 0;
			currentVelocity = 0;
			targetVelocity = 0;
		}
	}
}
