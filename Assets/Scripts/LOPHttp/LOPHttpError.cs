using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

/// <summary>
/// Error codes returned by PlayFabAPIs
/// </summary>
public enum LOPHttpErrorCode
{
    Unknown = 1,
    ConnectionError = 2,
    JsonParseError = 3,
    Success = 0,
    UnkownError = 500,
    ServiceUnavailable = 1123,
}

public class LOPHttpError
{
    public string ApiEndpoint;
    public int HttpCode;
    public string HttpStatus;
    public LOPHttpErrorCode Error;
    public string ErrorMessage;
    public Dictionary<string, List<string>> ErrorDetails;
    public object CustomData;

    public override string ToString()
    {
        return GenerateErrorReport();
    }

    [ThreadStatic]
    private static StringBuilder _tempSb;
    /// <summary>
    /// This converts the PlayFabError into a human readable string describing the error.
    /// If error is not found, it will return the http code, status, and error
    /// </summary>
    /// <returns>A description of the error that we just incur.</returns>
    public string GenerateErrorReport()
    {
        if (_tempSb == null)
            _tempSb = new StringBuilder();
        _tempSb.Length = 0;
        if (String.IsNullOrEmpty(ErrorMessage))
        {
            _tempSb.Append(ApiEndpoint).Append(": ").Append("Http Code: ").Append(HttpCode.ToString()).Append("\nHttp Status: ").Append(HttpStatus).Append("\nError: ").Append(Error.ToString()).Append("\n");
        }
        else
        {
            _tempSb.Append(ApiEndpoint).Append(": ").Append(ErrorMessage);
        }

        if (ErrorDetails != null)
            foreach (var pair in ErrorDetails)
                foreach (var msg in pair.Value)
                    _tempSb.Append("\n").Append(pair.Key).Append(": ").Append(msg);
        return _tempSb.ToString();
    }
}
