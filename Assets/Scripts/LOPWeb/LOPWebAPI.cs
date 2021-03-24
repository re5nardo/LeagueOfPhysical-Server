using System.Collections;
using System.Collections.Generic;
using System;

public class LOPWebAPI
{
    /// <summary>
    /// Send alive notification.
    /// </summary>
    public static void Alive()
    {
        LOPHttpTransport.Put(LOPHttp.GetFullUrl("/healthcheck/alive", null, LOPServerSettings.Get("LOPServerSettings")));
    }
}
