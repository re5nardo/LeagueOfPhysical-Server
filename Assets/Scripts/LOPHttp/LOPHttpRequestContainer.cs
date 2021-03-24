using System.Collections;
using System.Collections.Generic;
using System;

public class LOPHttpRequestContainer
{
    // This class stores the state of the request and all associated data
    public string ApiEndpoint = null;
    public string FullUrl = null;
    public byte[] Payload = null;
    public string JsonResponse = null;
    public LOPHttpRequestBase ApiRequest;
    public Dictionary<string, string> RequestHeaders;
    public LOPHttpResultBase ApiResult;
    public LOPHttpError Error;
    public Action DeserializeResultJson;
    public Action InvokeSuccessCallback;
    public Action<LOPHttpError> ErrorCallback;
    public object CustomData = null;
    public LOPServerSettings settings;
}
