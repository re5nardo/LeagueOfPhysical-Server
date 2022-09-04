using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System.Linq;

public class TickSyncController : LOPMonoSyncControllerBase<TickSyncData>
{
    public override SyncScope SyncScope { get; protected set; } = SyncScope.Global;

    private TickSyncData lastSyncData;

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
            if (lastSyncData == null || lastSyncData.ObjectToHash().SequenceEqual(GetSyncData().ObjectToHash()) == false)
            {
                var syncData = GetSyncData();
                Sync(syncData);

                lastSyncData = syncData;
            }
        }
    }

    public override TickSyncData GetSyncData()
    {
        return new TickSyncData(Game.Current.CurrentTick);
    }
}
