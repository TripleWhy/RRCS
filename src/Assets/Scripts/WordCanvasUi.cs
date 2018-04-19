namespace AssemblyCSharp
{
	using UnityEngine;

	public class WordCanvasUi : MonoBehaviour
	{
		public readonly CircuitManager manager = new CircuitManager();
		private float simuationSpeed = 1f;
		private bool paused = false;

		void FixedUpdate()
		{
			if (paused)
				Time.timeScale = 0f;
			manager.EvaluateIfNecessary();
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
			paused = false;
			Time.timeScale = SimulationSpeed;
		}

		public void SimulationPause()
		{
			paused = true;
			Time.timeScale = 0f;
		}

		public void SimulationStep()
		{
			paused = true;
			Time.timeScale = 1;
		}
	}
}