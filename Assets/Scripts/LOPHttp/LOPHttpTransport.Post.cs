using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public partial class LOPHttpTransport
{
    private IEnumerator PostRoutine(string uri, string postData, Dictionary<string, string> requestHeaders = null, Action<byte[]> onResult = null, Action<string> onError = null)
    {
        using (var www = UnityWebRequest.Post(uri, postData))
        {
            requestHeaders?.ForEach(headerPair =>
            {
                if (!string.IsNullOrEmpty(headerPair.Key) && !string.IsNullOrEmpty(headerPair.Value))
                {
                    www.SetRequestHeader(headerPair.Key, headerPair.Value);
                }
                else
                {
                    Debug.LogWarning("Null header: " + headerPair.Key + " = " + headerPair.Value);
                }
            });

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                onError?.Invoke(www.error);
            }
            else
            {
                onResult?.Invoke(www.downloadHandler.data);
            }
        }
    }
}
