using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using NetworkModel.Mirror;

public class EntityTransformController : LOPMonoEntityComponentBase
{
    private EntityTransformSnap entityTransformSnap = new EntityTransformSnap();
    private List<EntityTransformSnap> entityTransformSnaps = new List<EntityTransformSnap>();
    private AverageQueue latencies = new AverageQueue();

    protected override void OnAttached(IEntity entity)
    {
        SceneMessageBroker.AddSubscriber<EntityTransformSnap>(OnEntityTransformSnap).Where(snap => snap.entityId == Entity.EntityID);
        SceneMessageBroker.AddSubscriber<TickMessage.LateTickEnd>(OnLateTickEnd);
    }

    protected override void OnDetached()
    {
        SceneMessageBroker.RemoveSubscriber<EntityTransformSnap>(OnEntityTransformSnap);
        SceneMessageBroker.RemoveSubscriber<TickMessage.LateTickEnd>(OnLateTickEnd);
    }

    private void OnEntityTransformSnap(EntityTransformSnap entityTransformSnap)
    {
        if (Entity.HasAuthority)
        {
            return;
        }

        entityTransformSnaps.Add(entityTransformSnap);

        if (entityTransformSnaps.Count > 100)
        {
            entityTransformSnaps.RemoveRange(0, entityTransformSnaps.Count - 100);
        }

        latencies.Add((float)(Mirror.NetworkTime.time - entityTransformSnap.GameTime));

        //  Broadcast EntityTransformSnap
        var synchronization = ObjectPool.Instance.GetObject<SC_Synchronization>();
        synchronization.listSnap.Add(entityTransformSnap);

        RoomNetwork.Instance.SendToNear(synchronization, Entity.Position, LOP.Game.BROADCAST_SCOPE_RADIUS, instant: true);
    }

    private void OnLateTickEnd(TickMessage.LateTickEnd message)
    {
        if (Entity.HasAuthority)
        {
            var synchronization = ObjectPool.Instance.GetObject<SC_Synchronization>();
            synchronization.listSnap.Add(entityTransformSnap.Set(Entity));

            RoomNetwork.Instance.SendToNear(synchronization, Entity.Position, LOP.Game.BROADCAST_SCOPE_RADIUS, instant: true);
        }
    }

    private void LateUpdate()
    {
        if (!Entity.HasAuthority)
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

            Entity.Position = Vector3.Lerp(before.position, next.position, t);
            Entity.Rotation = Quaternion.Lerp(Quaternion.Euler(before.rotation), Quaternion.Euler(next.rotation), t).eulerAngles;
            Entity.Velocity = Vector3.Lerp(before.velocity, next.velocity, t);
            Entity.AngularVelocity = Vector3.Lerp(before.angularVelocity, next.angularVelocity, t);
        }
        else if (before != null)
        {
            float elapsed = syncTime - before.GameTime;

            Entity.Position = before.position + before.velocity * elapsed;
            Entity.Rotation = before.rotation;
            Entity.Velocity = before.velocity;
            Entity.AngularVelocity = before.angularVelocity;
        }
    }
}
