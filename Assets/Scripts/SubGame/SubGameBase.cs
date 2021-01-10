﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public abstract class SubGameBase : MonoBehaviour
{
    public static SubGameBase Current = null;

    public bool Initialized { get; private set; } = false;
    public bool IsGameEnd { get; protected set; } = false;

    protected int startTick = -1;

    private void Awake()
    {
        Current = this;
    }

    private void OnDestroy()
    {
        if (Current == this)
        {
            Current = null;
        }
    }

    public IEnumerator Initialize()
    {
        yield return OnInitialize();

        Initialized = true;
    }
    
    public void StartGame()
    {
        startTick = Game.Current.CurrentTick;

        IsGameEnd = false;

        TickPubSubService.AddSubscriber("Tick", OnTick);
        TickPubSubService.AddSubscriber("EarlyTickEnd", OnEarlyTickEnd);

        OnGameStart();

        OnTick(Game.Current.CurrentTick);
    }

    public void EndGame()
    {
        OnGameEnd();

        startTick = -1;

        IsGameEnd = true;

        TickPubSubService.RemoveSubscriber("Tick", OnTick);
        TickPubSubService.RemoveSubscriber("EarlyTickEnd", OnEarlyTickEnd);
    }

    protected abstract IEnumerator OnInitialize();
    protected abstract void OnGameStart();
    protected abstract void OnGameEnd();
    protected abstract void OnTick(int tick);
    protected abstract void OnEarlyTickEnd(int tick);
}