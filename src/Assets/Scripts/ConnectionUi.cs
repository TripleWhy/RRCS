using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class ConnectionUi : MonoBehaviour
	{
		protected Connection connection;

		public PortUi sourcePortUi;
		public PortUi targetPortUi;

		public Vector2 virtualTargetPosition;

		public Vector2 startPosition
		{
			get
			{
				return sourcePortUi.Center;
			}
		}

		public Vector2 targetPosition
		{
			get
			{
				if (targetPortUi)
					return targetPortUi.Center;
				return virtualTargetPosition;
			}
		}

		private LineRenderer lineRenderer
		{
			get
			{
				return GetComponent<LineRenderer>();
			}
		}

		public void SetVirtualTargetPosition(Vector2 pos)
		{
			virtualTargetPosition = pos;
			UpdatePositions();
		}

		public virtual void UpdatePositions()
		{
			lineRenderer.SetPosition(0, startPosition);
			lineRenderer.SetPosition(1, targetPosition);
		}

		public virtual void UpdateColors()
		{
			lineRenderer.startColor = sourcePortUi.Image.color;
			if (targetPortUi == null)
				lineRenderer.endColor = lineRenderer.startColor;
			else
				lineRenderer.endColor = targetPortUi.Image.color;
		}

		public virtual void Disconnect()
		{
			if (Connection != null)
			{
				Connection.Disconnect();
			}
			else
			{
				if (sourcePortUi != null)
					sourcePortUi.RemoveConnection(this, true);
			}
		}

		public virtual Connection Connection
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

				connection = value;
				UiManager.Register(this);
			}
		}

		void OnDestroy()
		{
			UiManager.Unregister(this);
		}
	}
}