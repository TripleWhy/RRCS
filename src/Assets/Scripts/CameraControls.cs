namespace AssemblyCSharp
{
	using UnityEngine;

	public class CameraControls : MonoBehaviour
	{
		public float inverseZoom = 0.5f;

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

			float wheel = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;
			if (!Mathf.Approximately(wheel, 0f))
			{
				inverseZoom *= Mathf.Exp(-10 * wheel);
				UpdateCameraZoom();
			}
		}

		private void UpdateCameraZoom()
		{
			cam.orthographicSize = inverseZoom * screenHeight * 0.5f;
		}
	}
}