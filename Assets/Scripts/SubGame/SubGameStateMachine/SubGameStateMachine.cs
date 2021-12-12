using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;

public class SubGameStateMachine : MonoStateMachineBase
{
    public override IState InitState => gameObject.GetOrAddComponent<SubGameState.SubGameEntryState>();

    private void Awake()
    {
        gameObject.AddComponent<SubGameStateMachineViewer>();
    }
}
