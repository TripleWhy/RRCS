namespace AssemblyCSharp
{
	using System.Text;
	using System.Collections;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.EventSystems;
	using SFB;
	using UnityEngine.Events;

	[RequireComponent(typeof(Button))]
	public class FileOpenButton : MonoBehaviour, IPointerDownHandler
	{
		[System.Serializable]
		public class FileSelectedEvent : UnityEvent<string, byte[]>
		{
		}

		[System.Serializable]
		public class FileSelectedTextEvent : UnityEvent<string, string>
		{
		}

		public string title = "";
		public string fileName = "";
		public string directory = "";
		public string extension = "";
		public bool multiselect = false;
		public bool compressed = true;
		public FileSelectedEvent onFileSelected;
		public FileSelectedTextEvent onFileSelectedText;

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string id);

	void Start()
	{
		if (onFileSelected == null)
			onFileSelected = new FileSelectedEvent();
	}

    public void OnPointerDown(PointerEventData eventData)
	{
        UploadFile(gameObject.name);
    }

    // Called from browser
    public void OnFileUploaded(string url)
	{
        FileSelected(url);
    }
#else
		//
		// Standalone platforms & editor
		//
		public void OnPointerDown(PointerEventData eventData) { }

		void Start()
		{
			var button = GetComponent<Button>();
			button.onClick.AddListener(OnClick);

			if (onFileSelected == null)
				onFileSelected = new FileSelectedEvent();
			if (onFileSelectedText == null)
				onFileSelectedText = new FileSelectedTextEvent();
		}

		private void OnClick()
		{
			string[] paths = StandaloneFileBrowser.OpenFilePanel(title, directory, extension, multiselect);
			if (paths.Length > 0)
				FileSelected(new System.Uri(paths[0]).AbsoluteUri);
		}
#endif

		private void FileSelected(string url)
		{
			StartCoroutine(LoadFile(url));
		}

		private IEnumerator LoadFile(string url)
		{
			WWW loader = new WWW(url);
			yield return loader;
			if (!compressed)
			{
				onFileSelected.Invoke(url, loader.bytes);
				onFileSelectedText.Invoke(url, loader.text);
			}
			else
			{
				byte[] data = FileUtils.UncompressDeflate(loader.bytes);
				onFileSelected.Invoke(url, data);
				onFileSelectedText.Invoke(url, Encoding.UTF8.GetString(data));
			}
		}
	}
}