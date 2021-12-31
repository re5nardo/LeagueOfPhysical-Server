using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class SubGameStateSyncController : LOPMonoSyncControllerBase<SubGameStateSyncData>
{
    public override SyncScope SyncScope { get; protected set; } = SyncScope.Global;

    private SubGameStateSyncData lastSyncData;

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

    public override SubGameStateSyncData GetSyncData()
    {
        return new SubGameStateSyncData(SubGameBase.Current.SubGameStateMachine.CurrentState.GetType().Name);
    }

    public override void OnSync(SyncDataEntry value)
    {
        if (HasAuthority)
        {
            return;
        }

        SyncSubGameState(value.data as SubGameStateSyncData);

        lastSyncData = value.data as SubGameStateSyncData;
    }

    private void SyncSubGameState(SubGameStateSyncData subGameStateSyncData)
    {
    }
}
