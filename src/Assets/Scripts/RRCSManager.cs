﻿namespace AssemblyCSharp
{
	using AssemblyCSharp.modals;
	using AssemblyCSharp.share;
	using UnityEngine;

	public class RRCSManager : MonoBehaviour
	{
		public readonly CircuitManager circuitManager = new CircuitManager();
		private float simuationSpeed = 1f;
		public Canvas WorldCanvas { get; private set; }
		public CameraControls cameraControls;
		public SelectionManager selectionManager;
		public NodeSettingsUi settingsEditor;
		public UnityEngine.UI.Button pauseButton;
		public int CurrentPlayerId { get; set; }
		public ShareFileModal shareFileModal;
		public LoadingModal loadingModal;
		public ErrorModal errorModal;

		public GameObject NodeUiPrefabRoot;

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


		public void Start()
		{
			ShareManager.Instance.ParseShareIdFromApplicationQuery();
		}

		private void Awake()
		{
			selectionManager.OnSelectionChange.AddListener(delegate
			{
				settingsEditor.SetSelectedNodes(selectionManager.GetSelectedNodes());
			});
			circuitManager.SimulationPauseRequested += SimulationPause;
		}

		void FixedUpdate()
		{
			circuitManager.Tick();
		}

		public float SimulationSpeed
		{
			get { return simuationSpeed; }
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
			pauseButton.interactable = true;
		}

		public void SimulationPause()
		{
			Time.timeScale = 0f;
			pauseButton.interactable = false;
		}

		public void SimulationStep()
		{
			SimulationPause();
			FixedUpdate();
		}

		public bool ShowPortLabels
		{
			get { return UiManager.ShowPortLabels; }
			set { UiManager.ShowPortLabels = value; }
		}

		public bool ShowEvaluationOrderLabels
		{
			get { return UiManager.ShowEvaluationOrderLabels; }
			set { UiManager.ShowEvaluationOrderLabels = value; }
		}

		public bool UseIntValuesOnly
		{
			get { return circuitManager.UseIntValuesOnly; }
			set { circuitManager.UseIntValuesOnly = value; }
		}

		public void ResetGizmos()
		{
			UiManager.ResetGizmos();
		}

		public void Clear()
		{
			// TODO: fix lines container
			var keepAlive = GameObject.Find("WorldCanvas/Lines").transform;
			foreach (Transform child in WorldCanvas.transform)
				if (child != keepAlive)
					Destroy(child.gameObject);
			//TODO reset camera
			circuitManager.Clear();
		}

		public void DbgSave()
		{
#if false
			StorageNodeGrahp container = new StorageNodeGrahp();
			container.Fill();
			string str = JsonUtility.ToJson(container, true);
			print(str);
			System.IO.File.WriteAllText(@"D:\Data\tmp\circuit.rrsc.json", str);
			FileUtils.StoreGZipFile(@"D:\Data\tmp\circuit.rrsc.gz", str);
			FileUtils.StoreDeflateFile(@"D:\Data\tmp\circuit.rrsc.deflate", str);
#endif
		}

		public void StoreFile(FileSaveButton button)
		{
			StorageNodeGrahp container = new StorageNodeGrahp();
			container.Fill();
			button.dataString = JsonUtility.ToJson(container, false);
		}

		public void LoadFile(string url, string content)
		{
			Clear();
			StorageNodeGrahp container = JsonUtility.FromJson<StorageNodeGrahp>(content);
			StartCoroutine(container.Restore(this));
		}

		public void ShareFile()
		{
			StorageNodeGrahp container = new StorageNodeGrahp();
			container.Fill();

			StartCoroutine(shareFileModal.Show(FileUtils.CompressGZipBase64(JsonUtility.ToJson(container, false))));
		}
	}
}
