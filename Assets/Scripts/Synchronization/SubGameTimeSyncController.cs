using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;
using System.Linq;

public class SubGameTimeSyncController : LOPMonoSyncControllerBase<SubGameTimeSyncData>
{
    public override SyncScope SyncScope { get; protected set; } = SyncScope.Global;

    private SubGameTimeSyncData lastSyncData;

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
        if (HasAuthority && SubGameBase.Current != null)
        {
            if (lastSyncData == null || lastSyncData.ObjectToHash().SequenceEqual(GetSyncData().ObjectToHash()) == false)
            {
                var syncData = GetSyncData();
                Sync(syncData);

                lastSyncData = syncData;
            }
        }
    }

    public override SubGameTimeSyncData GetSyncData()
    {
        return new SubGameTimeSyncData(SubGameBase.Current == null ? 0 : SubGameBase.Current.ElapsedTime);
    }
}
