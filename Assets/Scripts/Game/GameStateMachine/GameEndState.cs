﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;
using System.Linq;

public class GameEndState : MonoStateBase
{
    public override void Enter()
    {
        base.Enter();

        //  send result to players
        //  ...
        SC_GameEnd gameEnd = new SC_GameEnd();
        gameEnd.winnerEntityIds = new List<int>();
        gameEnd.loserEntityIds = new List<int>();
        gameEnd.rankingDataList = new List<RankingData>();

        RoomNetwork.Instance.SendToAll(gameEnd);

        //  send result to web server
        var matchSettingData = AppDataContainer.Get<MatchSettingData>();

        MatchEndRequest request = new MatchEndRequest();
        request.matchId = PhotonNetwork.room.Name;
        request.matchType = matchSettingData.matchSetting.matchType.ToString();
        request.subGameId = matchSettingData.matchSetting.subGameId;
        request.mapId = matchSettingData.matchSetting.mapId;

        request.playerIds = LOP.Game.Current.EntityIDPlayerUserID.Values.ToList();
        request.winnerPlayerIds = null;
        request.loserPlayerIds = null;
        request.rankingDataList = null;

        LOPWebAPI.MatchEnd(request,
            result =>
            {
                if (result.code == 200)
                {
                    LOP.Application.Quit();
                }
            },
            error =>
            {
            }
        );
    }

    public override IState GetNext<I>(I input)
    {
        if (!Enum.TryParse(input.ToString(), out GameStateInput gameStateInput))
        {
            Debug.LogError($"Invalid input! input : {input}");
            return default;
        }

        switch (gameStateInput)
        {
            case GameStateInput.StateDone:
                return gameObject.GetOrAddComponent<GameEndState>();
        }

        throw new Exception($"Invalid transition: {GetType().Name} with {gameStateInput}");
    }
}
