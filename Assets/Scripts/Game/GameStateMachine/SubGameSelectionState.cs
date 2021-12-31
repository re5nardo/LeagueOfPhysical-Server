using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;

namespace GameState
{
    public class SubGameSelectionState : MonoStateBase
    {
        public override void OnEnter()
        {
            var subGameDatas = SubGameData.GetAll();

            //var subGameIndex = UnityEngine.Random.Range(0, subGameDatas.Length);
            //var mapIndex = UnityEngine.Random.Range(0, subGameDatas[subGameIndex].availableMapIds.Length);

            //LOP.Game.Current.GameManager.subGameId = subGameDatas[subGameIndex].subGameId;
            //LOP.Game.Current.GameManager.mapId = subGameDatas[subGameIndex].availableMapIds[mapIndex];

            AppDataContainer.Get<MatchSettingData>().matchSetting.subGameId = "FlapWang";
            AppDataContainer.Get<MatchSettingData>().matchSetting.mapId = "FlapWangMap";

            //LOP.Game.Current.GameManager.subGameId = "FallingGame";
            //LOP.Game.Current.GameManager.mapId = "Falling";

            FSM.MoveNext(GameStateInput.StateDone);
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
                case GameStateInput.StateDone: return gameObject.GetOrAddComponent<GameState.SubGamePrepareState>();
            }

            throw new Exception($"Invalid transition: {GetType().Name} with {gameStateInput}");
        }
    }
}
