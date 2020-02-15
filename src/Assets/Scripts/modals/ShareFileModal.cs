using System;
using System.Collections;
using System.Text;
using AssemblyCSharp.share;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp.modals
{
    public class ShareFileModal : Modal
    {
        public ShareSuccessModal shareSuccessModal;
        public InputField nameInput;
        public InputField descriptionInput;
        public Button submitButton;
        public Button closeButton;
        public Button loadButton;
        public RectTransform leftSidebarTransform;
        public RectTransform rightSidebarTransform;
        public Canvas uiCanvas;
        public RawImage thumbnailPreview;
        public InputField currentUrlInput;
        public Text currentUrlTitle;

        private string dataToSubmit;

        private string thumbnailToSubmit;
        private Texture _previewTex;

        public void Start()
        {
            submitButton.onClick.AddListener(OnSubmit);
            closeButton.onClick.AddListener(Hide);
            loadButton.onClick.AddListener(OnLoad);
        }

        public IEnumerator Show(string data)
        {
            nameInput.text = "";
            descriptionInput.text = "";
            dataToSubmit = data;

            // Create Thumbnail
            yield return new WaitForEndOfFrame();

            int leftSideBarWidth =
                45 + (int) (RectTransformUtility.PixelAdjustRect(leftSidebarTransform, uiCanvas).width *
                           uiCanvas.transform.localScale.x);
            int rightSideBarWidth =
                45 + (int) (RectTransformUtility.PixelAdjustRect(rightSidebarTransform, uiCanvas).width *
                           uiCanvas.transform.localScale.x);

            int width = Screen.width - leftSideBarWidth - rightSideBarWidth;
            int height = Screen.height;
            
            if (width < 0)
            {
                // Fallback Image if sidebars overlap
                width = 10;
                height = 10;
                leftSideBarWidth = 0;
            }

            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(leftSideBarWidth, 0, width, height), 0, 0);
            tex.Apply();

            setThumbnailPreviewTex(tex);

            var bytes = tex.EncodeToJPG();


            thumbnailToSubmit = Convert.ToBase64String(bytes);

            if (ShareManager.Instance.lastLoadedId.Length > 0)
            {
                currentUrlTitle.text = "Current Share Link:";
                currentUrlInput.text = ShareManager.SHARE_BASE_URL + ShareManager.Instance.lastLoadedId;
            }
            else
                currentUrlTitle.text = "Load Share Link:";

            Show();
        }

        public void OnLoad()
        {
            string id = currentUrlInput.text;

            id = id.Replace(" ", "");
            id = id.Replace("?", "");
            id = id.Replace("circuit=", "");
            id = id.Replace(ShareManager.SHARE_BASE_URL, "");
            string[] parts = id.Split('/');
            id = parts[parts.Length - 1];

            if (id.Length == 0)
                RRCSManager.Instance.errorModal.Show("Error:", "Could not parse circuit id from provided url.");
            else
            {
                id = parts[parts.Length - 1];

                Hide();
                RRCSManager.Instance.StartCoroutine(ShareManager.Instance.LoadShareId(id));
            }
        }

        public void OnSubmit()
        {
            RRCSManager.Instance.StartCoroutine(SubmitShareRequest());
            RRCSManager.Instance.loadingModal.Show("Uploading...");
            Hide();
        }

        IEnumerator SubmitShareRequest()
        {
            ShareRequest request = new ShareRequest
            {
                data = dataToSubmit,
                description = descriptionInput.text,
                name = nameInput.text,
                thumbnail = thumbnailToSubmit
            };
            var www = ShareManager.Instance.UploadShareRequest(request);
            yield return www;

            RRCSManager.Instance.loadingModal.Hide();
            if (www.error != null)
            {
                Debug.Log("Upload failed: " + www.error);
                RRCSManager.Instance.errorModal.Show("Upload failed: ", www.error);
            }
            else
            {
                if (nameInput.text.Length > 0)
                    ShareManager.Instance.SetLastLoadedId(www.text, nameInput.text);
                else
                    ShareManager.Instance.SetLastLoadedId(www.text, "Unnamed Circuit");
                
                shareSuccessModal.Show(ShareManager.SHARE_BASE_URL + www.text);
            }
        }

        private void setThumbnailPreviewTex(Texture tex)
        {
            if (_previewTex != null)
            {
                Destroy(_previewTex);
            }

            thumbnailPreview.texture = tex;
            thumbnailPreview.GetComponent<AspectRatioFitter>().aspectRatio = (float) tex.width / tex.height;
            _previewTex = tex;
        }
    }
}