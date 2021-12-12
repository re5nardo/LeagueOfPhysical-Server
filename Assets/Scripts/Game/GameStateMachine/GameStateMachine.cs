using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;

public class GameStateMachine : MonoStateMachineBase
{
    public override IState InitState => gameObject.GetOrAddComponent<GameState.GameEntryState>();

    private void Awake()
    {
        gameObject.AddComponent<GameStateMachineViewer>();
    }
}
