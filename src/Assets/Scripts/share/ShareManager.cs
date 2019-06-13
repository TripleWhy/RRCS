using System;
using System.Collections;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace AssemblyCSharp.share
{
    public class ShareManager
    {
        public static readonly string BLOB_STORE_URL = "https://rrcs.tk/rrcs-blob/";
        public static readonly string SHARE_BASE_URL = "https://rrcs.tk/circuit/";

        public string lastLoadedId = "";
        private static ShareManager instance;

        public static ShareManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ShareManager();
                return instance;
            }
        }

        public void ParseShareIdFromApplicationQuery()
        {
            Debug.Log("absoluteUrl: " + Application.absoluteURL);
            if (Application.absoluteURL != null)
            {
                string[] urlParts = Application.absoluteURL.Split('?');
                if (urlParts.Length > 1)
                {
                    string[] queries = urlParts[1].Split('&');

                    foreach (string query in queries)
                    {
                        string[] queryParts = query.Split('=');
                        if (queryParts.Length == 2)
                        {
                            if (queryParts[0] == "id")
                            {
                                RRCSManager.Instance.StartCoroutine(LoadShareId(queryParts[1]));
                            }
                        }
                    }
                }
            }
        }

        public IEnumerator LoadShareId(string id)
        {
            RRCSManager.Instance.loadingModal.Show("Downloading shared file...");
            yield return 0;
            Debug.Log("Fetching Blob: " + id);

            UnityWebRequest www = UnityWebRequest.Get(BLOB_STORE_URL + id);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("Network Error: " + www.error);
                RRCSManager.Instance.errorModal.Show("Network Error:",
                    www.error + "\n" + Encoding.UTF8.GetString(www.downloadHandler.data));
            }
            else
            {
                Debug.Log("Got Blob Data:");
                Debug.Log(Encoding.UTF8.GetString(www.downloadHandler.data));

                ShareBlob blob = null;
                try
                {
                    blob = JsonUtility.FromJson<ShareBlob>(Encoding.UTF8.GetString(www.downloadHandler.data));
                } catch (ArgumentException ex)
                {
                    RRCSManager.Instance.errorModal.Show("Invalid JSON: ", ex.Message);
                    Debug.Log("Invalid JSON: " + ex.Message);
                    Debug.Log(ex.StackTrace);
                }

                if (blob != null && blob.dataUrl != null)
                {
                    www = UnityWebRequest.Get(blob.dataUrl);
                    yield return www.SendWebRequest();

                    if (www.isNetworkError || www.isHttpError)
                    {
                        Debug.Log("Network Error: " + www.error);
                        RRCSManager.Instance.errorModal.Show("Network Error:", www.error);
                    }
                    else
                    {
                        lastLoadedId = id;
                        RRCSManager.Instance.LoadFile(null, FileUtils.UncompressGZip(www.downloadHandler.data));
                    }
                }
                else
                {
                    RRCSManager.Instance.errorModal.Show("Network Error:", "dataUrl is empty! No file loaded.");
                    Debug.Log("dataUrl is empty! No file loaded.");
                }
            }


            RRCSManager.Instance.loadingModal.Hide();
        }

        public WWW UploadShareRequest(ShareRequest request)
        {
            WWW www;
            Hashtable postHeader = new Hashtable();
            postHeader.Add("Content-Type", "application/json");

            var formData = Encoding.UTF8.GetBytes(JsonUtility.ToJson(request));

            www = new WWW(BLOB_STORE_URL, formData, postHeader);
            return www;
        }
    }
}