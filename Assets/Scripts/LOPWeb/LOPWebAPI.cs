﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class LOPWebAPI
{
    public static WebRequest<string> Heartbeat(string roomId)
    {
        return new WebRequestBuilder<string>()
            .SetUri($"{LOP.Application.Env.roomBaseURL}/room/heartbeat/{roomId}")
            .SetMethod(HttpMethod.PUT)
            .Build();
    }

    public static WebRequest<string> NotifyStartServer(NotifyStartServerRequest request)
    {
        return new WebRequestBuilder<string>()
            .SetUri($"{LOP.Application.Env.roomBaseURL}/room")
            .SetMethod(HttpMethod.PUT)
            .SetRequestBody(request)
            .Build();
    }

    public static WebRequest<string> NotifyStopServer(string roomId)
    {
        return new WebRequestBuilder<string>()
            .SetUri($"{LOP.Application.Env.roomBaseURL}/room/{roomId}")
            .SetMethod(HttpMethod.DELETE)
            .Build();
    }

    public static WebRequest<UpdateRoomStatusResult> UpdateRoomStatus(UpdateRoomStatusRequest request)
    {
        return new WebRequestBuilder<UpdateRoomStatusResult>()
            .SetUri($"{LOP.Application.Env.roomBaseURL}/room/status")
            .SetMethod(HttpMethod.PUT)
            .SetRequestBody(request)
            .Build();
    }

    #region Match
    public static WebRequest<GetMatchResult> GetMatch(string matchId)
    {
        return new WebRequestBuilder<GetMatchResult>()
            .SetUri($"{LOP.Application.Env.roomBaseURL}/match/{matchId}")
            .SetMethod(HttpMethod.GET)
            .Build();
    }

    public static WebRequest<MatchStartResult> MatchStart(MatchStartRequest request)
    {
        return new WebRequestBuilder<MatchStartResult>()
            .SetUri($"{LOP.Application.Env.roomBaseURL}/match/match-start")
            .SetMethod(HttpMethod.PUT)
            .SetRequestBody(request)
            .Build();
    }

    public static WebRequest<MatchEndResult> MatchEnd(MatchEndRequest request)
    {
        return new WebRequestBuilder<MatchEndResult>()
            .SetUri($"{LOP.Application.Env.roomBaseURL}/match/match-end")
            .SetMethod(HttpMethod.PUT)
            .SetRequestBody(request)
            .Build();
    }
    #endregion
}
