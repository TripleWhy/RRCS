namespace AssemblyCSharp
{
	using UnityEngine;

	public class RRCSManager : MonoBehaviour
	{
		public readonly CircuitManager circuitManager = new CircuitManager();
		private float simuationSpeed = 1f;
		internal CameraControls cameraControls;

		private static RRCSManager instance;
		public static RRCSManager Instance
		{
			get
			{
				if (instance == null)
					instance = GameObject.FindObjectOfType<RRCSManager>();
				return instance;
			}
		}

		void FixedUpdate()
		{
			circuitManager.EvaluateIfNecessary();
		}

		public float SimulationSpeed
		{
			get
			{
				return simuationSpeed;
			}
			set
			{
				if (value <= 0f)
					return;
				simuationSpeed = value;
				if (Time.timeScale > 0f)
					Time.timeScale = value;
			}
		}

		public void SetSimulationSpeed(string inputString)
		{
			float speed;
			if (float.TryParse(inputString, out speed))
				SimulationSpeed = speed;
		}

		public void SimulationPlay()
		{
			Time.timeScale = SimulationSpeed;
		}

		public void SimulationPause()
		{
			Time.timeScale = 0f;
		}

		public void SimulationStep()
		{
			SimulationPause();
			FixedUpdate();
		}

		public bool ShowPortLabels
		{
			get
			{
				return UiManager.ShowPortLabels;
			}
			set
			{
				UiManager.ShowPortLabels = value;
			}
		}
	}
}