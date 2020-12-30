using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;

public class SubGameProgressState : MonoBehaviour, IState<GameStateInput>
{
    public IFiniteStateMachine<IState<GameStateInput>, GameStateInput> FSM => gameObject.GetOrAddComponent<GameStateMachine>();

    public void Enter()
    {
        SubGameBase.Current.StartGame();
    }

    public void Execute()
    {
        if (SubGameBase.Current.IsGameEnd)
        {
            FSM.MoveNext(GameStateInput.StateDone);
        }
    }

    public void Exit()
    {
    }

    public IState<GameStateInput> GetNext(GameStateInput input)
    {
        switch (input)
        {
            case GameStateInput.StateDone:
                return gameObject.GetOrAddComponent<SubGameClearState>();
        }

        throw new Exception($"Invalid transition: {GetType().Name} with {input}");
    }
}
