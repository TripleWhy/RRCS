using UnityEngine;

namespace AssemblyCSharp.gizmos
{
	public class Rotator : Gizmo
	{
		private float currentPosition = 0; // deg
		private float currentVelocity = 0; // deg/s

		private float targetVelocity = 0; // rpm
		private float currentMaxAcceleration = 0; // deg/(s^2)

		public Rotator(CircuitManager manager) : base(manager, 3)
		{
		}

		protected override void EvaluateOutputs()
		{
			if (InBool(0))
			{
				int accelerationTime = GetAccelerationTime();
				float inputVelocity = InFloat(1); // cm/s

				if (inputVelocity == 0 && IsMoveToTarget())
				{
					// Instant
					currentPosition = Mathf.Repeat(InFloat(2), 360); ;
					currentVelocity = 0;
					targetVelocity = 0;
				}
				else
				{
					if (accelerationTime == 0)
					{
						// 1 RPM = 6 deg/s
						currentVelocity = inputVelocity * 6f;
					}
					else
					{
						if (targetVelocity != inputVelocity)
						{
							targetVelocity = inputVelocity;
							currentMaxAcceleration =
								Mathf.Abs(targetVelocity * 6f - currentVelocity) * (1f / accelerationTime);
						}

						currentVelocity += Mathf.Clamp((targetVelocity * 6f - currentVelocity),
							-currentMaxAcceleration, currentMaxAcceleration);
					}

					if (IsMoveToTarget())
					{
						float clampPositionMin;
						float clampPositionMax;
						float targetPosition = Mathf.Repeat(InFloat(2), 360);

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
							targetVelocity = 0;
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

		public override string GetGizmoValueString()
		{
			if (GetAccelerationTime() > 0)
			{
				return string.Format("{0:0.0}° @{1:0.0}°/s", currentPosition, currentVelocity);
			}
			else
			{
				return string.Format("{0:0.0}°", currentPosition);
			}
		}

		private int GetAccelerationTime()
		{
			int val = (int) settings[0].currentValue;
			return val < 0 ? 0 : val;
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
