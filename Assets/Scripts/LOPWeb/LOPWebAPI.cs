using System.Collections;
using System.Collections.Generic;

public class LOPWebAPI
{
    /// <summary>
    /// Send alive notification.
    /// </summary>
    public static void Alive(string roomId)
    {
        GameFramework.HttpTransport.Put(GameFramework.Http.GetFullUri($"/healthcheck/alive/{roomId}", null, GameFramework.ServerSettings.Get("ServerSettings_Room")));
    }
}
