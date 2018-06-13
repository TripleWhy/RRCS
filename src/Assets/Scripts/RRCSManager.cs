namespace AssemblyCSharp
{
	using UnityEngine;
	using UnityEngine.UI.Extensions;

	public class RRCSManager : MonoBehaviour
	{
		public readonly CircuitManager circuitManager = new CircuitManager();
		private float simuationSpeed = 1f;
		public Canvas WorldCanvas { get; private set; }
		public CameraControls cameraControls;
		public SelectionManager selectionManager;
		public NodeSettingsUi settingsEditor;
		public int CurrentPlayerId { get; set; }

		private static RRCSManager instance;
		public static RRCSManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = GameObject.FindObjectOfType<RRCSManager>();
					instance.CurrentPlayerId = 1;
					instance.WorldCanvas = instance.GetComponent<Canvas>();
				}
				return instance;
			}
		}

		private void Awake()
		{
			selectionManager.OnSelectionChange.AddListener(delegate { settingsEditor.SetSelectedNodes(selectionManager.GetSelectedNodes()); });
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

		public void Clear()
		{
			foreach (Transform child in WorldCanvas.transform)
				Destroy(child.gameObject);
			//TODO reset camera
			circuitManager.Clear();
		}

		public void StoreFile(FileSaveButton button)
		{
			//button.data = ...
		}

		public void LoadFile(string url)
		{
		}
	}
}