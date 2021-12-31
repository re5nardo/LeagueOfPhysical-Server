using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;

public class SubGameStateMachine : MonoStateMachineBase
{
    public override IState InitState => gameObject.GetOrAddComponent<SubGameState.EntryState>();

    private SubGameStateSyncController subGameStateSyncController;

    private void Awake()
    {
        gameObject.AddComponent<SubGameStateMachineViewer>();
        subGameStateSyncController = gameObject.AddComponent<SubGameStateSyncController>();

        StartStateMachine();
    }

    public override void OnStateChange()
    {
        base.OnStateChange();

        subGameStateSyncController.Sync(subGameStateSyncController.GetSyncData());
    }
}
