using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;

public class GameStateMachine : MonoBehaviour, IFiniteStateMachine<IState<GameStateInput>, GameStateInput>
{
    public IState<GameStateInput> InitState => gameObject.GetOrAddComponent<WaitForPlayersState>();
    public IState<GameStateInput> CurrentState { get; private set; }

    public void StartStateMachine()
    {
        CurrentState = InitState;
        CurrentState.Enter();
    }
 
    private void Update()
    {
        CurrentState?.Execute();
    }

    public IState<GameStateInput> MoveNext(GameStateInput input)
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
}
