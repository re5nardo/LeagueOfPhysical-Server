using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System.Linq;

public class TransformSyncController : LOPMonoEntitySyncControllerBase<TransformSyncData>
{
    private TransformSyncData transformSyncData = new TransformSyncData();
    private List<SyncDataEntry> syncDataEntries = new List<SyncDataEntry>();
    private AverageQueue latencies = new AverageQueue();
    private TransformSyncData lastSyncData;

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

    public override TransformSyncData GetSyncData()
    {
        return transformSyncData.Set(Entity);
    }

    public override void OnSync(SyncDataEntry value)
    {
        if (HasAuthority)
        {
            return;
        }

        syncDataEntries.Add(value);

        int possession = 20;
        if (syncDataEntries.Count > possession)
        {
            syncDataEntries.RemoveRange(0, syncDataEntries.Count - possession);
        }

        latencies.Add((float)(Mirror.NetworkTime.time - value.GameTime()));

        lastSyncData = value.data as TransformSyncData;
    }

    private void LateUpdate()
    {
        if (!HasAuthority)
        {
            SyncTransform();
        }
    }

    private void SyncTransform()
    {
        if (syncDataEntries.Count == 0)
        {
            return;
        }

        double syncTime = Mirror.NetworkTime.time - latencies.Average - 0.01f;

        var beforeEntry = syncDataEntries.FindLast(x => x.GameTime() <= syncTime);
        var nextEntry = syncDataEntries.Find(x => x.GameTime() >= syncTime);

        var before = beforeEntry.data as TransformSyncData;
        var next = nextEntry.data as TransformSyncData;

        if (before != null && next != null)
        {
            float t = (float)((beforeEntry.GameTime() == nextEntry.GameTime()) ? 0 : (syncTime - beforeEntry.GameTime()) / (nextEntry.GameTime() - beforeEntry.GameTime()));

            Entity.Position = Vector3.Lerp(before.position, next.position, t);
            Entity.Rotation = Quaternion.Lerp(Quaternion.Euler(before.rotation), Quaternion.Euler(next.rotation), t).eulerAngles;
            Entity.Velocity = Vector3.Lerp(before.velocity, next.velocity, t);
            Entity.AngularVelocity = Vector3.Lerp(before.angularVelocity, next.angularVelocity, t);
        }
        else if (before != null)
        {
            float elapsed = (float)(syncTime - beforeEntry.GameTime());

            Entity.Position = before.position + before.velocity * elapsed;
            Entity.Rotation = before.rotation;
            Entity.Velocity = before.velocity;
            Entity.AngularVelocity = before.angularVelocity;
        }
    }
}
