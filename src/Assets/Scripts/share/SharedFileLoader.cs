using System;
using System.Collections;
using System.Text;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityEngine.Networking;

namespace AssemblyCSharp.share
{
    public class SharedFileLoader : MonoBehaviour
    {
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
                                StartCoroutine(LoadShareId(queryParts[1]));
                            }
                        }
                    }
                }
            }
        }

        IEnumerator LoadShareId(string id)
        {
            yield return 0;
            Debug.Log("Fetching Blob: " + id);
            RRCSManager.Instance.loadingModal.Show("Downloading shared file...");

            UnityWebRequest www = UnityWebRequest.Get(ShareFileModal.BLOB_STORE_URL + id);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("Network Error: " + www.error);
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
                    }
                    else
                    {
                        RRCSManager.Instance.LoadFile(null, Encoding.UTF8.GetString(www.downloadHandler.data));
                    }
                }
                else
                {
                    Debug.Log("dataUrl is empty! No file loaded.");
                }
            }


            RRCSManager.Instance.loadingModal.Hide();
        }
    }
}