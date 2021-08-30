using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;
using NetworkModel.Mirror;
using GameFramework;

public class TransformController : MonoBehaviour
{
    private MonoEntityBase monoEntity;
    private RoomProtocolDispatcher roomProtocolDispatcher;
    private List<EntityTransformSnap> entityTransformSnaps = new List<EntityTransformSnap>();
    private AverageQueue latencies = new AverageQueue();

    private void Awake()
    {
        monoEntity = GetComponent<MonoEntityBase>();

        roomProtocolDispatcher = gameObject.AddComponent<RoomProtocolDispatcher>();
        roomProtocolDispatcher[typeof(CS_Synchronization)] = OnCS_Synchronization;
    }

    private void OnCS_Synchronization(IMessage msg)
    {
        if (monoEntity.HasAuthority)
        {
            return;
        }

        CS_Synchronization synchronization = msg as CS_Synchronization;

        synchronization.listSnap?.ForEach(snap =>
        {
            if (snap is EntityTransformSnap entityTransformSnap && entityTransformSnap.entityId == monoEntity.EntityID)
            {
                entityTransformSnaps.Add(entityTransformSnap);

                if (entityTransformSnaps.Count > 100)
                {
                    entityTransformSnaps.RemoveRange(0, entityTransformSnaps.Count - 100);
                }

                latencies.Add((float)(Mirror.NetworkTime.time - entityTransformSnap.GameTime));

                //  Broadcast EntityTransformSnap
                var synchronization = ObjectPool.Instance.GetObject<SC_Synchronization>();
                synchronization.listSnap.Add(entityTransformSnap);

                RoomNetwork.Instance.SendToAll(synchronization, instant: true);
            }
        });
    }

    private void LateUpdate()
    {
        if (!monoEntity.HasAuthority)
        {
            SyncTransform();
        }
    }

    private void SyncTransform()
    {
        if (entityTransformSnaps.Count == 0)
        {
            return;
        }

        float syncTime = (float)Mirror.NetworkTime.time - latencies.Average - 0.01f;

        EntityTransformSnap before = entityTransformSnaps.FindLast(x => x.GameTime <= syncTime);
        EntityTransformSnap next = entityTransformSnaps.Find(x => x.GameTime >= syncTime);

        if (before != null && next != null)
        {
            float t = (before.GameTime == next.GameTime) ? 0 : (syncTime - before.GameTime) / (next.GameTime - before.GameTime);

            monoEntity.Position = Vector3.Lerp(before.position, next.position, t);
            monoEntity.Rotation = Quaternion.Lerp(Quaternion.Euler(before.rotation), Quaternion.Euler(next.rotation), t).eulerAngles;
            monoEntity.Velocity = Vector3.Lerp(before.velocity, next.velocity, t);
            monoEntity.AngularVelocity = Vector3.Lerp(before.angularVelocity, next.angularVelocity, t);
        }
        else if (before != null)
        {
            float elapsed = syncTime - before.GameTime;

            monoEntity.Position = before.position + before.velocity * elapsed;
            monoEntity.Rotation = before.rotation;
            monoEntity.Velocity = before.velocity;
            monoEntity.AngularVelocity = before.angularVelocity;
        }
    }
}
