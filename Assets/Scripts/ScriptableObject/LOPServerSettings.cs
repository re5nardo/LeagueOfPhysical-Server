using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LOPServerSettings", menuName = "ScriptableObjects/LOPServerSettings", order = 1)]
public class LOPServerSettings : ScriptableObjectWrapper<LOPServerSettings>
{
    public string scheme;
    public string host;
    public int port;

    public string GetFullUrl(string apiCall, Dictionary<string, string> getParams)
    {
        return LOPHttp.GetFullUrl(apiCall, getParams, this);
    }
}
