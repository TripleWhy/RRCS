namespace AssemblyCSharp
{
	using System;
	using UnityEngine;

	public class CameraControls : MonoBehaviour
	{
		private float inverseZoom = 0.5f;

		private Camera cam;
		private int screenHeight;

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

			if (Input.GetMouseButton(2))
			{
				float speed = cam.orthographicSize;
				transform.position -= new Vector3(speed * Input.GetAxis("Mouse X") * Time.deltaTime, speed * Input.GetAxis("Mouse Y") * Time.deltaTime, 0f);
			}

			float wheel = Input.GetAxis("Mouse ScrollWheel");
			if (!Mathf.Approximately(wheel, 0f))
			{
				if (wheel > 0)
					InverseZoom *= 0.5f;
				else
					InverseZoom *= 2f;
			}
		}

		public float InverseZoom
		{
			get
			{
				return inverseZoom;
			}
			set
			{
				float inv = Math.Max(Math.Min(8f, value), 0.0625f);
				if (inv == inverseZoom)
					return;
				inverseZoom = inv;
				UpdateCameraZoom();
			}
		}

		private void UpdateCameraZoom()
		{
			cam.orthographicSize = inverseZoom * screenHeight * 0.5f;
		}
	}
}