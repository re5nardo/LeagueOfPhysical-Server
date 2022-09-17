using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

public class LOPWebAPI
{
    public static HttpRequestContainer<string> Heartbeat(string roomId, Action<string> onResult = null, Action<string> onError = null)
    {
        return Http.Put($"room/heartbeat/{roomId}", onResult, onError, apiSettings: GameFramework.ServerSettings.Get("ServerSettings_Room"));
    }

    public static HttpRequestContainer<string> NotifyStartServer(NotifyStartServerRequest request, Action<string> onResult = null, Action<string> onError = null)
    {
        return Http.Put($"room", JsonUtility.ToJson(request), onResult, onError, apiSettings: GameFramework.ServerSettings.Get("ServerSettings_Room"));
    }

    public static HttpRequestContainer<string> NotifyStopServer(string roomId, Action<string> onResult = null, Action<string> onError = null)
    {
        return Http.Delete($"room/{roomId}", onResult, onError, apiSettings: GameFramework.ServerSettings.Get("ServerSettings_Room"));
    }

    public static HttpRequestContainer<UpdateRoomStatusResult> UpdateRoomStatus(UpdateRoomStatusRequest request, Action<UpdateRoomStatusResult> onResult = null, Action<string> onError = null)
    {
        return Http.Put($"room/status", JsonUtility.ToJson(request), onResult, onError, apiSettings: GameFramework.ServerSettings.Get("ServerSettings_Room"));
    }

    #region Match
    public static HttpRequestContainer<GetMatchResult> GetMatch(string matchId, Action<GetMatchResult> onResult = null, Action<string> onError = null)
    {
        return Http.Get($"match/{matchId}", onResult, onError, apiSettings: GameFramework.ServerSettings.Get("ServerSettings_Room"));
    }

    public static HttpRequestContainer<MatchStartResult> MatchStart(MatchStartRequest request, Action<MatchStartResult> onResult = null, Action<string> onError = null)
    {
        return Http.Put($"match/match-start", JsonUtility.ToJson(request), onResult, onError, apiSettings: GameFramework.ServerSettings.Get("ServerSettings_Room"));
    }

    public static HttpRequestContainer<MatchEndResult> MatchEnd(MatchEndRequest request, Action<MatchEndResult> onResult = null, Action<string> onError = null)
    {
        return Http.Put($"match/match-end", JsonUtility.ToJson(request), onResult, onError, apiSettings: GameFramework.ServerSettings.Get("ServerSettings_Room"));
    }
    #endregion
}
