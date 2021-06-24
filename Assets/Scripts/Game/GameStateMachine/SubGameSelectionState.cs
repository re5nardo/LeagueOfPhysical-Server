using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;

public class SubGameSelectionState : MonoStateBase
{
    public override void Enter()
    {
        base.Enter();

        var subGameDatas = SubGameData.GetAll();

        var subGameIndex = UnityEngine.Random.Range(0, subGameDatas.Length);
        var mapIndex = UnityEngine.Random.Range(0, subGameDatas[subGameIndex].availableMapNames.Length);

        LOP.Game.Current.GameManager.subGameId = subGameDatas[subGameIndex].subGameId;
        LOP.Game.Current.GameManager.mapName = subGameDatas[subGameIndex].availableMapNames[mapIndex];


        LOP.Game.Current.GameManager.subGameId = "JumpWang";
        LOP.Game.Current.GameManager.mapName = "Space";


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
            case GameStateInput.StateDone:
                return gameObject.GetOrAddComponent<SubGamePrepareState>();
        }

        throw new Exception($"Invalid transition: {GetType().Name} with {gameStateInput}");
    }
}
