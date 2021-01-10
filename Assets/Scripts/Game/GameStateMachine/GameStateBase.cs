using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;

public abstract class GameStateBase : MonoBehaviour, IState
{
    public IFiniteStateMachine FSM => gameObject.GetOrAddComponent<GameStateMachine>();

    public virtual void Enter()
    {
    }

    public virtual void Execute()
    {
    }

    public virtual void Exit()
    {
    }

    public abstract IState GetNext<I>(I input) where I : Enum;
}
