using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;

public abstract class GameStateBase : MonoBehaviour, IState
{
    public IFiniteStateMachine FSM => gameObject.GetOrAddComponent<GameStateMachine>();

    protected RoomProtocolDispatcher roomProtocolDispatcher = null;

    private void Awake()
    {
        roomProtocolDispatcher = gameObject.AddComponent<RoomProtocolDispatcher>();
    }

    public void Enter()
    {
        OnEnter();
    }

    public void Execute()
    {
        OnExecute();
    }

    public void Exit()
    {
        OnExit();

        roomProtocolDispatcher.Clear();
    }

    protected virtual void OnEnter()
    {
    }

    protected virtual void OnExecute()
    {
    }

    protected virtual void OnExit()
    {
    }

    public abstract IState GetNext<I>(I input) where I : Enum;
}
