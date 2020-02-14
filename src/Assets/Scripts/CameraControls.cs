using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AssemblyCSharp
{
	using System;
	using UnityEngine;

	public class CameraControls : MonoBehaviour
	{
		private const float zoomSpeed = 1.25f;
		private const int minZoomLevel = -12;
		private const int maxZoomLevel = 10;
		private float inverseZoom = 0.5f;
		private float zoomLevel = 0;
		private Vector3 lastMousePosition;

		private Camera cam;
		private int screenHeight;

		private bool isPinchPanZoomActive = false;
		Vector2 lastPinchCenter = Vector2.zero;
		private float lastPinchDistance = 0;
		private int[] pinchFingerIds = new int[] {-1, -1};

		public delegate void ZoomChangedEventHandler(float inverseZoom);

		public event ZoomChangedEventHandler ZoomChanged = delegate { };

		void Awake()
		{
			cam = GetComponent<Camera>();
		}

		void Update()
		{
			if (Screen.height != screenHeight)
			{
				screenHeight = Screen.height;
				UpdateCameraZoom();
			}

			if (IsMouseOnWorldCanvas())
			{
				if (Input.GetButtonDown("PanCamera"))
				{
					lastMousePosition = Input.mousePosition;
				}
				else if (Input.GetButton("PanCamera"))
				{
					transform.position += (lastMousePosition - Input.mousePosition) * InverseZoom;
					lastMousePosition = Input.mousePosition;
				}

				float wheel = Input.GetAxis("Mouse ScrollWheel");
				if (!Mathf.Approximately(wheel, 0f))
				{
					var mousePos = cam.ScreenToViewportPoint(Input.mousePosition);

					if (mousePos.x > 0 && mousePos.y > 0 && mousePos.x < 1 && mousePos.y < 1)
					{
						if (wheel > 0 && ZoomLevel >= maxZoomLevel)
							return;
						else if (wheel < 0 && ZoomLevel <= minZoomLevel)
							return;

						if (wheel > 0)
						{
							ZoomToPosition(Input.mousePosition, 1);
						}
						else
						{
							ZoomToPosition(Input.mousePosition, -1);
						}
					}
				}
			}

			// Pan/Zoom functionality for touchscreen devices
			if (Input.touchCount > 1)
			{
				Touch touch0 = Input.GetTouch(0);
				Touch touch1 = Input.GetTouch(1);
				float pinchDistance = Vector2.Distance(touch0.position, touch1.position);
				Vector2 pinchCenter = (touch0.position + touch1.position) / 2;

				if (!isPinchPanZoomActive || pinchFingerIds[0] != touch0.fingerId ||
				    pinchFingerIds[1] != touch1.fingerId)
				{
					isPinchPanZoomActive = true;
					lastPinchCenter = pinchCenter;
					lastPinchDistance = pinchDistance;
					pinchFingerIds[0] = touch0.fingerId;
					pinchFingerIds[1] = touch1.fingerId;
					RRCSManager.Instance.selectionManager.SelectionEnabled = false;
				}

				Vector2 pinchCenterDiff = lastPinchCenter - pinchCenter;
				transform.position +=
					new Vector3(pinchCenterDiff.x, pinchCenterDiff.y) * InverseZoom;

				float distanceDiff = pinchDistance - lastPinchDistance;
				ZoomToPosition(pinchCenter, distanceDiff * pinchZoomSpeed);

				lastPinchCenter = pinchCenter;
				lastPinchDistance = pinchDistance;
			}

			else
			{
				if (isPinchPanZoomActive)
				{
					RRCSManager.Instance.selectionManager.SelectionEnabled = true;
					isPinchPanZoomActive = false;
				}
			}
		}

		public void ZoomToPosition(Vector3 zoomCenter, float zoom)
		{
			//TODO: get sidebar widths?
			const int leftBarWidth = 0;
			const int rightBarWidth = 0;

			Vector3 screenCenter = new Vector3((Screen.width + leftBarWidth - rightBarWidth) / 2f,
				Screen.height / 2f);

			Vector3 translation = (zoomCenter - screenCenter) / (4f * zoomSpeed);
			transform.position += zoom * translation * InverseZoom;
			ZoomLevel += zoom;
		}

		public float InverseZoom
		{
			get { return inverseZoom; }
		}

		public float ZoomLevel
		{
			get { return zoomLevel; }

			set
			{
				float lvl = Math.Max(Math.Min(maxZoomLevel, value), minZoomLevel);
				if (lvl == zoomLevel)
					return;
				zoomLevel = lvl;
				inverseZoom = (float) (0.5 * Math.Pow(zoomSpeed, -lvl));
				UpdateCameraZoom();
				ZoomChanged(inverseZoom);
			}
		}

		private void UpdateCameraZoom()
		{
			cam.orthographicSize = inverseZoom * screenHeight * 0.5f;
		}

		public bool IsMouseOnWorldCanvas()
		{
			PointerEventData ped = new PointerEventData(null);
			ped.position = Input.mousePosition;
			List<RaycastResult> results = new List<RaycastResult>();

			EventSystem.current.RaycastAll(ped, results);

			if (results.Count > 0)
				if (results[0].gameObject.transform.root.gameObject == RRCSManager.Instance.WorldCanvas.gameObject)
					return true;


			return false;
		}
	}
}