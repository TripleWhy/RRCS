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
				cam.orthographicSize = inverseZoom * screenHeight * 0.5f;
			}

			if (Input.GetMouseButton(2))
			{
				float speed = cam.orthographicSize;
				transform.position -= new Vector3(speed * Input.GetAxis("Mouse X") * Time.deltaTime, speed * Input.GetAxis("Mouse Y") * Time.deltaTime, 0f);
			}
		}
	}
}