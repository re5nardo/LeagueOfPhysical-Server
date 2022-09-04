using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;
using System.Linq;
using NetworkModel.Mirror;
using Mirror;
using GameFramework;

namespace GameState
{
    public class EndState : MonoStateBase
    {
        public override void OnEnter()
        {
            //  send result to players
            //  ...
            using var disposer = PoolObjectDisposer<SC_GameEnd>.Get();
            var gameEnd = disposer.PoolObject;
            gameEnd.listWinnerEntityId = new List<int>();
            gameEnd.listLoserEntityId = new List<int>();
            gameEnd.listRankingData = new List<RankingData>();

            RoomNetwork.Instance.SendToAll(gameEnd);

            //  send result to web server
            var matchEndRequest = new MatchEndRequest()
            {
                matchId = SceneDataContainer.Get<MatchData>().matchId,
                matchSetting = SceneDataContainer.Get<MatchData>().matchSetting,

                playerIds = GameIdMap.UserIds.ToList(),
                winnerPlayerIds = null,
                loserPlayerIds = null,
                rankingDataList = null,
            };

            //LOPWebAPI.MatchEnd(matchEndRequest,
            //    result =>
            //    {
            //        if (!IsCurrent) return;

            //        if (result.code == 200)
            //        {
            //        }
            //    },
            //    error =>
            //    {
            //    }
            //);
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
            }

            throw new Exception($"Invalid transition: {GetType().Name} with {gameStateInput}");
        }
    }
}
