using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;

public class GameStateMachine : MonoBehaviour, IFiniteStateMachine
{
    public IState InitState => gameObject.GetOrAddComponent<WaitForPlayersState>();
    public IState CurrentState { get; private set; }

    public void StartStateMachine()
    {
        CurrentState = InitState;
        CurrentState.Enter();
    }
 
    private void Update()
    {
        CurrentState?.Execute();
    }

    public IState MoveNext<I>(I input) where I : Enum
    {
        var next = CurrentState.GetNext(input);

        if (CurrentState == next)
        {
            return CurrentState;
        }

        CurrentState.Exit();

        CurrentState = next;

        CurrentState.Enter();

        return CurrentState;
    }
}

public enum GameStateInput
{
    StateDone = 0,

    //  States
    WaitForPlayersState = 100,
    SubGameSelectionState = 101,
    SubGamePrepareState = 102,
    SubGameProgressState = 103,
    SubGameEndState = 104,
    MatchEndState = 105,
}
