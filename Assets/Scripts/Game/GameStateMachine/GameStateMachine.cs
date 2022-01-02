using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;

public class GameStateMachine : MonoStateMachineBase
{
    public override IState InitState => gameObject.GetOrAddComponent<GameState.EntryState>();

    private GameStateSyncController gameStateSyncController;

    private void Awake()
    {
        gameObject.AddComponent<GameStateMachineViewer>();
        gameStateSyncController = gameObject.AddComponent<GameStateSyncController>();
    }

    public override void OnStateChange()
    {
        base.OnStateChange();

        gameStateSyncController.Sync(gameStateSyncController.GetSyncData());
    }
}
