using System.Collections;
using System.Collections.Generic;

public class LOPWebAPI
{
    /// <summary>
    /// Send alive notification.
    /// </summary>
    public static void Alive(string roomId)
    {
        var getParams = new Dictionary<string, string>();
        getParams.Add("roomId", roomId);

        LOPHttpTransport.Put(LOPHttp.GetFullUrl("/healthcheck/alive", getParams, LOPServerSettings.Get("LOPServerSettings")));
    }
}
