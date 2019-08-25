using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace AssemblyCSharp
{
	public class TransitionConnectionUi : ConnectionUi
	{
		private static readonly Vector2 UP = new Vector2(0, 1);
		private static readonly Vector3 delayIconOrientation = new Vector3(0, 0, 90);
		public PortUi enabledPort;

		public LineRenderer lineRenderer
		{
			get
			{
				return GetComponent<LineRenderer>();
			}
		}

		public override void UpdatePositions()
		{
			int positionCount = lineRenderer.positionCount;

			Vector2 diff = (targetPosition - startPosition);
			Vector2 offsetVec = (diff.normalized * 15);
			Vector2 orthogonalOffset = new Vector2(offsetVec.y, -offsetVec.x);
			Vector2 portPosition = startPosition + (diff * 0.5f) + orthogonalOffset;


			for (int i = 0; i < positionCount; i++)
			{
				float t = (float) i / (positionCount - 1);
				float tOrthogonal = (0.5f - Mathf.Abs(t - 0.5f)) * 2f;
				tOrthogonal = Mathf.Pow(tOrthogonal, 0.5f);

				lineRenderer.SetPosition(i, startPosition + diff * t + orthogonalOffset * tOrthogonal);
			}

			if (enabledPort != null)
			{
				enabledPort.transform.position = new Vector3(portPosition.x, portPosition.y, enabledPort.transform.position.z);
				Vector3 transformEulerAngles = enabledPort.transform.eulerAngles;
				transformEulerAngles.z = -Vector2.SignedAngle(diff, UP);
				enabledPort.transform.eulerAngles = transformEulerAngles;

				enabledPort.valueText.transform.position = new Vector3(portPosition.x - 16, portPosition.y, enabledPort.transform.position.z);

				if (enabledPort.delayIcon != null)
					enabledPort.delayIcon.transform.eulerAngles = delayIconOrientation;
			}
		}

		public override Connection Connection
		{
			get
			{
				return connection;
			}
			set
			{
				if (connection != null)
				{
					if (object.ReferenceEquals(connection, value))
						return;
					throw new InvalidOperationException();
				}

				if (enabledPort != null)
					enabledPort.Port = ((StateMachineTransition)value).TransitionEnabledPort;
				connection = value;
				UiManager.Register(this);
			}
		}

		public override void Disconnect()
		{
			if (enabledPort != null && enabledPort.Port != null)
				enabledPort.Port.DisconnectAll();
			base.Disconnect();
		}
	}
}