using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class SynchronizationManager : MonoSingleton<SynchronizationManager>
{
    private List<ISnap> snaps = new List<ISnap>();

    protected override void Awake()
    {
        base.Awake();

        TickPubSubService.AddSubscriber("LateTickEnd", OnLateTickEnd);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        TickPubSubService.RemoveSubscriber("LateTickEnd", OnLateTickEnd);
    }

    public static void SendSnap(ISnap snap)
    {
        Instance.SendSnapInternal(snap.Clone());
    }

    private void SendSnapInternal(ISnap snap)
    {
        if (snap == null)
        {
            Debug.LogWarning("snap is null!");
            return;
        }

        snaps.Add(snap);
    }

    private void OnLateTickEnd(int tick)
    {
        if (snaps.Count > 0)
        {
            SC_Synchronization synchronization = ObjectPool.Instance.GetObject<SC_Synchronization>();
            synchronization.snaps = snaps;

            RoomNetwork.Instance.SendToAll(synchronization, true, true);

            ObjectPool.Instance.ReturnObject(synchronization);

            snaps.Clear();
        }
    }
}
