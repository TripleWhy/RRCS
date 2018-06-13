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

	[RequireComponent(typeof(Button))]
	public class FileSaveButton : MonoBehaviour, IPointerDownHandler
	{
		[System.Serializable]
		public class FileSelectedEvent : UnityEvent<FileSaveButton>
		{
		}

		public string Title = "";
		public string Directory = "";
		public string FileName = "";
		public string Extension = "";

		public FileSelectedEvent onClicked;
		public byte[] data;

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
        DownloadFile(gameObject.name, FileName + "." + Extension, data, data.Length);
		data = null;
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
			var path = StandaloneFileBrowser.SaveFilePanel(Title, Directory, FileName, Extension);
			if (!string.IsNullOrEmpty(path))
				File.WriteAllBytes(path, data);
			data = null;
		}
#endif

		private void ButtonClicked()
		{
			onClicked.Invoke(this);
		}
	}
}