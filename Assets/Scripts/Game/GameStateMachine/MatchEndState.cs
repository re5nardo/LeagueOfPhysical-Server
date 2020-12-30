using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;

public class MatchEndState : MonoBehaviour, IState<GameStateInput>
{
    public IFiniteStateMachine<IState<GameStateInput>, GameStateInput> FSM => gameObject.GetOrAddComponent<GameStateMachine>();

    public void Enter()
    {
        Debug.Log("MatchEndState Enter");
    }

    public void Execute()
    {
    }

    public void Exit()
    {
    }

    public IState<GameStateInput> GetNext(GameStateInput input)
    {
        switch (input)
        {
            case GameStateInput.StateDone:
                return gameObject.GetOrAddComponent<MatchEndState>();
        }

        throw new Exception($"Invalid transition: {GetType().Name} with {input}");
    }
}
