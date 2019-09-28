using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AssemblyCSharp
{
	using System;
	using UnityEngine;

	public class CameraControls : MonoBehaviour
	{
		private float inverseZoom = 0.5f;
		private int zoomLevel = 0;

		private Camera cam;
		private int screenHeight;

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
				if (Input.GetButton("PanCamera"))
				{
					float speed = cam.orthographicSize * 2.0f;
					transform.position -= new Vector3(speed * Input.GetAxis("Mouse X") * Time.deltaTime,
						speed * Input.GetAxis("Mouse Y") * Time.deltaTime, 0f);
				}

				float wheel = Input.GetAxis("Mouse ScrollWheel");
				if (!Mathf.Approximately(wheel, 0f))
				{
					var mousePos = cam.ScreenToViewportPoint(Input.mousePosition);

					if (mousePos.x > 0 && mousePos.y > 0 && mousePos.x < 1 && mousePos.y < 1)
					{
						if (wheel > 0)
							ZoomLevel++;
						else
							ZoomLevel--;
					}
				}
			}
		}

		public float InverseZoom
		{
			get { return inverseZoom; }
		}

		public int ZoomLevel
		{
			get { return zoomLevel; }
			set
			{
				int lvl = Math.Max(Math.Min(10, value), -12);
				if (lvl == zoomLevel)
					return;
				zoomLevel = lvl;
				inverseZoom = (float)(0.5 * Math.Pow(1.25, -lvl));
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