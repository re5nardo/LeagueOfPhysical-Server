using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;

public class SubGameSelectionState : GameStateBase
{
    private SubGameData[] subGameDatas = null;

    protected override void OnEnter()
    {
        subGameDatas = Resources.LoadAll<SubGameData>("ScriptableObject/SubGameData");

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
