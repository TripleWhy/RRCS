namespace AssemblyCSharp
{
	using System.IO;
	using System.Text;
	using System.Runtime.InteropServices;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.EventSystems;
	using SFB;
	using UnityEngine.Events;
	using System;

	[RequireComponent(typeof(Button))]
	public class FileSaveButton : MonoBehaviour, IPointerDownHandler
	{
		[System.Serializable]
		public class FileSelectedEvent : UnityEvent<FileSaveButton>
		{
		}

		public string title = "";
		public string directory = "";
		public string fileName = "";
		public string extension = "";
		public bool compress = true;

		public FileSelectedEvent onClicked;
		[NonSerialized] public byte[] dataBytes;
		[NonSerialized] public string dataString;

#if UNITY_WEBGL && !UNITY_EDITOR
		//
		// WebGL
		//
		[DllImport("__Internal")]
		private static extern void DownloadFile(string id, string filename, byte[] byteArray, int byteArraySize);

		void Start()
		{
			if (onClicked == null)
				onClicked = new FileSelectedEvent();
		}

		// Broser plugin should be called in OnPointerDown.
		public void OnPointerDown(PointerEventData eventData)
		{
			ButtonClicked();
			byte[] data = CreateBytes();
			DownloadFile(gameObject.name, fileName + "." + extension, data, data.Length);
			dataBytes = null;
			dataString = null;
		}

		// Called from browser
		public void OnFileDownloaded()
		{
			//
		}
#else
		//
		// Standalone platforms & editor
		//
		public void OnPointerDown(PointerEventData eventData) { }

		// Listen OnClick event in standlone builds
		void Start()
		{
			var button = GetComponent<Button>();
			button.onClick.AddListener(OnClick);

			if (onClicked == null)
				onClicked = new FileSelectedEvent();
		}

		public void OnClick()
		{
			ButtonClicked();
			string path = StandaloneFileBrowser.SaveFilePanel(title, directory, fileName, extension);
			if (!string.IsNullOrEmpty(path))
				FileUtils.StoreFile(path, CreateBytes(), false);
			dataBytes = null;
			dataString = null;
		}
#endif
		private byte[] CreateBytes()
		{
			if (dataBytes != null)
				return FileUtils.StorageBytes(dataBytes, compress);
			else
				return FileUtils.StorageBytes(dataString, compress);
		}

		private void ButtonClicked()
		{
			onClicked.Invoke(this);
		}
	}
}