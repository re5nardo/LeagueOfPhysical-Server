using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using GameFramework;
using System.IO;
using System.IO.Compression;

/// <summary>
/// This is a wrapper for Http So we can better separate the functionaity of Http Requests delegated to WWW or HttpWebRequest
/// </summary>
public class LOPHttp : MonoSingleton<LOPHttp>
{
    private LOPHttpTransport httpTransport = null;

    protected override void Awake()
    {
        base.Awake();

        httpTransport = Instance.gameObject.AddComponent<LOPHttpTransport>();
    }

    public void Start()
    {
        DontDestroyOnLoad(this);
    }

    #region Static functions
    public static void MakeApiCall<TResult>(string apiEndpoint, LOPHttpRequestBase request, Action<TResult> resultCallback, Action<LOPHttpError> errorCallback,
        object customData = null, Dictionary<string, string> extraHeaders = null, LOPServerSettings apiSettings = null) where TResult : LOPHttpResultBase
    {
        var reqContainer = new LOPHttpRequestContainer
        {
            ApiEndpoint = apiEndpoint,
            FullUrl = apiSettings.GetFullUrl(apiEndpoint, null),
            settings = apiSettings ?? LOPServerSettings.Get(),
            CustomData = customData,
            Payload = Encoding.UTF8.GetBytes(JsonUtility.ToJson(request)),
            ApiRequest = request,
            ErrorCallback = errorCallback,
            RequestHeaders = extraHeaders ?? new Dictionary<string, string>(),
        };

        // These closures preserve the TResult generic information in a way that's safe for all the devices
        reqContainer.DeserializeResultJson = () =>
        {
            reqContainer.ApiResult = JsonUtility.FromJson<TResult>(reqContainer.JsonResponse);
        };
        reqContainer.InvokeSuccessCallback = () =>
        {
            resultCallback?.Invoke((TResult)reqContainer.ApiResult);
        };

        MakeApiCall(reqContainer);
    }

    public static void MakeApiCall(LOPHttpRequestContainer reqContainer)
    {
        reqContainer.RequestHeaders["Content-Type"] = "application/json";
        reqContainer.RequestHeaders["Accept-Encoding"] = "gzip";

        bool compress = reqContainer.Payload.Length > 1024;
        if (compress)
        {
            reqContainer.RequestHeaders["Content-Encoding"] = "gzip";

            using (var memoryStream = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    gZipStream.Write(reqContainer.Payload, 0, reqContainer.Payload.Length);
                }
                reqContainer.Payload = memoryStream.ToArray();
            }
        }

        LOPHttpTransport.Put(reqContainer.FullUrl, reqContainer.Payload, reqContainer.RequestHeaders,
            (string result) =>
            {
                Instance.OnResponse(result, reqContainer);
            },
            error =>
            {
                Instance.OnError(error, reqContainer);
            }
        );
    }

    public static string GetFullUrl(string apiCall, Dictionary<string, string> getParams, LOPServerSettings apiSettings)
    {
        StringBuilder sb = new StringBuilder(1000);

        sb.Append(apiSettings.scheme).Append("://")
            .Append(apiSettings.host).Append(":")
            .Append(apiSettings.port)
            .Append(apiCall);

        if (getParams != null)
        {
            bool firstParam = true;
            foreach (var paramPair in getParams)
            {
                if (firstParam)
                {
                    sb.Append("?");
                    firstParam = false;
                }
                else
                {
                    sb.Append("&");
                }
                sb.Append(paramPair.Key).Append("=").Append(paramPair.Value);
            }
        }

        return sb.ToString();
    }
    #endregion

    private LOPHttpError GenerateLOPHttpError(string apiEndpoint, string json, object customData)
    {
        Dictionary<string, object> errorDict = null;
        Dictionary<string, List<string>> errorDetails = null;

        try
        {
            // Deserialize the error
            //errorDict = serializer.DeserializeObject<Dictionary<string, object>>(json);
        }
        catch (Exception) { /* Unusual, but shouldn't actually matter */ }
        try
        {
            object errorDetailsString;
            //if (errorDict != null && errorDict.TryGetValue("errorDetails", out errorDetailsString))
            //    errorDetails = serializer.DeserializeObject<Dictionary<string, List<string>>>(errorDetailsString.ToString());
        }
        catch (Exception) { /* Unusual, but shouldn't actually matter */ }

        return new LOPHttpError
        {
            ApiEndpoint = apiEndpoint,
            HttpCode = errorDict != null && errorDict.ContainsKey("code") ? Convert.ToInt32(errorDict["code"]) : 400,
            HttpStatus = errorDict != null && errorDict.ContainsKey("status") ? (string)errorDict["status"] : "BadRequest",
            Error = errorDict != null && errorDict.ContainsKey("errorCode") ? (LOPHttpErrorCode)Convert.ToInt32(errorDict["errorCode"]) : LOPHttpErrorCode.ServiceUnavailable,
            ErrorMessage = errorDict != null && errorDict.ContainsKey("errorMessage") ? (string)errorDict["errorMessage"] : json,
            ErrorDetails = errorDetails,
            CustomData = customData
        };
    }

    private void OnResponse(string jsonResponse, LOPHttpRequestContainer reqContainer)
    {
        try
        {
            var LOPHttpResult = JsonUtility.FromJson<LOPHttpResultBase>(jsonResponse);

            if (LOPHttpResult.code == 200)
            {
                reqContainer.JsonResponse = jsonResponse;
                reqContainer.DeserializeResultJson();
                reqContainer.ApiResult.Request = reqContainer.ApiRequest;
                reqContainer.ApiResult.CustomData = reqContainer.CustomData;
                reqContainer.InvokeSuccessCallback();
            }
            else
            {
                if (reqContainer.ErrorCallback != null)
                {
                    reqContainer.Error = GenerateLOPHttpError(reqContainer.ApiEndpoint, jsonResponse, reqContainer.CustomData);
                    reqContainer.ErrorCallback(reqContainer.Error);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void OnError(string error, LOPHttpRequestContainer reqContainer)
    {
        reqContainer.JsonResponse = error;

        if (reqContainer.ErrorCallback != null)
        {
            reqContainer.Error = GenerateLOPHttpError(reqContainer.ApiEndpoint, reqContainer.JsonResponse, reqContainer.CustomData);
            reqContainer.ErrorCallback(reqContainer.Error);
        }
    }
}
