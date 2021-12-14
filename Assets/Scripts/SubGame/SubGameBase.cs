using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

[DisallowMultipleComponent]
public abstract class SubGameBase : MonoBehaviour
{
    public static SubGameBase Current = null;

    public bool Initialized { get; private set; } = false;
    public bool Finalized { get; private set; } = false;
    public bool IsGameEnd { get; protected set; } = false;

    protected int startTick;
    protected float ElapsedTime => (Game.Current.CurrentTick - startTick) * Game.Current.TickInterval;

    public SubGameStateMachine SubGameStateMachine { get; private set; }

    private void Awake()
    {
        Current = this;

        SubGameStateMachine = new GameObject("SubGameStateMachine").AddComponent<SubGameStateMachine>();
    }

    private void OnDestroy()
    {
        if (Current == this)
        {
            Current = null;
        }

        if (SubGameStateMachine != null)
        {
            Destroy(SubGameStateMachine.gameObject);
        }
    }

    public IEnumerator Initialize()
    {
        yield return OnInitialize();

        Initialized = true;
    }

    public IEnumerator Finalize()
    {
        yield return OnFinalize();

        Finalized = true;
    }

    public void StartGame()
    {
        startTick = Game.Current.CurrentTick;

        IsGameEnd = false;

        SceneMessageBroker.AddSubscriber<TickMessage.Tick>(OnTickMessage);
        SceneMessageBroker.AddSubscriber<TickMessage.EarlyTickEnd>(OnEarlyTickEndMessage);

        OnGameStart();

        OnTick(Game.Current.CurrentTick);
    }

    public void EndGame()
    {
        OnGameEnd();

        startTick = 0;

        IsGameEnd = true;

        SceneMessageBroker.RemoveSubscriber<TickMessage.Tick>(OnTickMessage);
        SceneMessageBroker.RemoveSubscriber<TickMessage.EarlyTickEnd>(OnEarlyTickEndMessage);
    }

    private void OnTickMessage(TickMessage.Tick message)
    {
        OnTick(message.tick);
    }

    private void OnEarlyTickEndMessage(TickMessage.EarlyTickEnd message)
    {
        OnEarlyTickEnd(message.tick);
    }

    protected abstract IEnumerator OnInitialize();
    protected abstract IEnumerator OnFinalize();

    protected abstract void OnGameStart();
    protected abstract void OnGameEnd();
    protected abstract void OnTick(int tick);
    protected abstract void OnEarlyTickEnd(int tick);
}
