using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using NetworkModel.Mirror;

public class SynchronizationManager : MonoSingleton<SynchronizationManager>
{
    private List<ISnap> snaps = new List<ISnap>();

    protected override void Awake()
    {
        base.Awake();

        SceneMessageBroker.AddSubscriber<TickMessage.LateTickEnd>(OnLateTickEnd);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        SceneMessageBroker.RemoveSubscriber<TickMessage.LateTickEnd>(OnLateTickEnd);
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

    private void OnLateTickEnd(TickMessage.LateTickEnd message)
    {
        if (snaps.Count > 0)
        {
            var synchronization = ObjectPool.Instance.GetObject<SC_Synchronization>();
            synchronization.listSnap = snaps;

            RoomNetwork.Instance.SendToAll(synchronization, true, true);

            ObjectPool.Instance.ReturnObject(synchronization);

            snaps.Clear();
        }
    }
}
