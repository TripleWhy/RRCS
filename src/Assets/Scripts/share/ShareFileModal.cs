using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp.share
{
    public class ShareFileModal : Modal
    {
        public static string BLOB_STORE_URL = "https://rrcs-243222.appspot.com/rrcs-blob/";
        public static string SHARE_BASE_URL = "https://triplewhy.gitlab.io/RRCS/?id=";

        public ShareSuccessModal shareSuccessModal;
        public InputField nameInput;
        public InputField descriptionInput;
        public Button submitButton;
        public Button closeButton;
        public RectTransform leftSidebarTransform;
        public RectTransform rightSidebarTransform;
        public Canvas uiCanvas;

        private string dataToSubmit;
        private string thumbnailToSubmit;

        public void Start()
        {
            submitButton.onClick.AddListener(OnSubmit);
            closeButton.onClick.AddListener(Hide);
        }

        public IEnumerator Show(string data)
        {
            nameInput.text = "";
            descriptionInput.text = "";
            dataToSubmit = data;

            // Create Thumbnail
            yield return new WaitForEndOfFrame();

            int leftSideBarWidth =
                1 + (int) (RectTransformUtility.PixelAdjustRect(leftSidebarTransform, uiCanvas).width *
                           uiCanvas.transform.localScale.x);
            int rightSideBarWidth =
                1 + (int) (RectTransformUtility.PixelAdjustRect(rightSidebarTransform, uiCanvas).width *
                           uiCanvas.transform.localScale.x);

            int width = Screen.width - leftSideBarWidth - rightSideBarWidth;
            int height = Screen.height;

            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(leftSideBarWidth, 0, width, height), 0, 0);
            tex.Apply();

            Debug.Log(width);
            Debug.Log(height);
            Debug.Log(leftSideBarWidth);
            Debug.Log(rightSideBarWidth);
            Debug.Log(Screen.width);
            Debug.Log(Screen.height);

            var bytes = tex.EncodeToJPG();
            Destroy(tex);

            thumbnailToSubmit = Convert.ToBase64String(bytes);

            Debug.Log(bytes.Length);
            Debug.Log(thumbnailToSubmit.Length);

            Show();
        }

        public void OnSubmit()
        {
            WWW www;
            Hashtable postHeader = new Hashtable();
            postHeader.Add("Content-Type", "application/json");

            // convert json string to byte
            var formData = Encoding.UTF8.GetBytes(JsonUtility.ToJson(new ShareRequest
            {
                data = dataToSubmit,
                description = descriptionInput.text,
                name = nameInput.text,
                thumbnail = thumbnailToSubmit
            }));
            Debug.Log(thumbnailToSubmit.Length);

            www = new WWW(BLOB_STORE_URL, formData, postHeader);
            StartCoroutine(WaitForRequest(www));

            RRCSManager.Instance.loadingModal.Show("Uploading...");
        }

        IEnumerator WaitForRequest(WWW data)
        {
            yield return data; // Wait until the download is done
            RRCSManager.Instance.loadingModal.Hide();
            Hide();
            if (data.error != null)
            {
                Debug.Log("Upload failed: " + data.error);
            }
            else
            {
                shareSuccessModal.Show(SHARE_BASE_URL + data.text);
            }
        }
    }
}