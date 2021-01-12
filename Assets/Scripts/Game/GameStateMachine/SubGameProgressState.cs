using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;

public class SubGameProgressState : GameStateBase
{
    protected override void OnEnter()
    {
        SubGameBase.Current.StartGame();
    }

    protected override void OnExecute()
    {
        if (SubGameBase.Current.IsGameEnd)
        {
            FSM.MoveNext(GameStateInput.StateDone);
        }
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
                return gameObject.GetOrAddComponent<SubGameClearState>();
        }

        throw new Exception($"Invalid transition: {GetType().Name} with {gameStateInput}");
    }
}
