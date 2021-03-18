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

        var index = UnityEngine.Random.Range(0, subGameDatas.Length);

        LOP.Game.Current.GameManager.currentSubGame = subGameDatas[index];

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
