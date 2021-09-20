using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkModel.Mirror;

public class CS_SynchronizationHandler
{
    public static void Handle(CS_Synchronization synchronization)
    {
        synchronization.listSnap?.ForEach(snap =>
        {
            SceneMessageBroker.Publish(snap.GetType(), snap);
        });
    }
}
