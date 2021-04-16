﻿using System.Collections;
using System.Collections.Generic;
using GameFramework;
using System;

public class LOPWebAPI
{
    public static HttpRequestContainer<string> Alive(string roomId, Action<string> onResult = null, Action<string> onError = null)
    {
        return Http.Put($"/healthcheck/alive/{roomId}", onResult, onError, apiSettings: GameFramework.ServerSettings.Get("ServerSettings_Room"));
    }
}
