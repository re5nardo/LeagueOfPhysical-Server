using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class GameStateSyncController : LOPMonoSyncControllerBase<GameStateSyncData>
{
    public override SyncScope SyncScope { get; protected set; } = SyncScope.Global;

    private GameStateSyncData lastSyncData;

    public override void OnInitialize()
    {
        base.OnInitialize();

        SceneMessageBroker.AddSubscriber<TickMessage.LateTickEnd>(OnLateTickEnd);
    }

    public override void OnFinalize()
    {
        base.OnFinalize();

        SceneMessageBroker.RemoveSubscriber<TickMessage.LateTickEnd>(OnLateTickEnd);
    }

    private void OnLateTickEnd(TickMessage.LateTickEnd message)
    {
        if (HasAuthority)
        {
            if (lastSyncData == null || lastSyncData.ObjectToHash() != GetSyncData().ObjectToHash())
            {
                var syncData = GetSyncData();
                Sync(syncData);

                lastSyncData = syncData;
            }
        }
    }

    public override GameStateSyncData GetSyncData()
    {
        return new GameStateSyncData(LOP.Game.Current.GameStateMachine.CurrentState.GetType().Name);
    }

    public override void OnSync(SyncDataEntry value)
    {
        if (HasAuthority)
        {
            return;
        }

        SyncGameState(value.data as GameStateSyncData);

        lastSyncData = value.data as GameStateSyncData;
    }

    private void SyncGameState(GameStateSyncData gameStateSyncData)
    {
    }
}