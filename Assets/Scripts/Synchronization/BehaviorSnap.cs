using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;
using Behavior;

[Serializable]
public class BehaviorSnap : ISnap
{
    public int Tick { get; set; }
    public int entityId;
    public int behaviorMasterId;

    public BehaviorSnap() { }
    public BehaviorSnap(BehaviorBase behavior)
    {
        Tick = Game.Current.CurrentTick;
        entityId = behavior.Entity.EntityID;
        behaviorMasterId = behavior.GetBehaviorMasterID();
    }

    public virtual bool EqualsCore(ISnap snap)
    {
        BehaviorSnap other = snap as BehaviorSnap;

        if (other == null) return false;
        if (other.entityId != entityId) return false;
        if (other.behaviorMasterId != behaviorMasterId) return false;

        return true;
    }

    public virtual bool EqualsValue(ISnap snap)
    {
        BehaviorSnap other = snap as BehaviorSnap;

        if (other == null) return false;
        if (other.entityId != entityId) return false;
        if (other.behaviorMasterId != behaviorMasterId) return false;

        return true;
    }

    public virtual ISnap Set(ISynchronizable synchronizable)
    {
        var behaviorBase = synchronizable as BehaviorBase;

        Tick = Game.Current.CurrentTick;
        entityId = behaviorBase.Entity.EntityID;
        behaviorMasterId = behaviorBase.GetBehaviorMasterID();

        return this;
    }

    public virtual ISnap Clone()
    {
        BehaviorSnap clone = new BehaviorSnap();

        clone.Tick = Tick;
        clone.entityId = entityId;
        clone.behaviorMasterId = behaviorMasterId;

        return clone;
    }

    public override string ToString()
    {
        return $"[Tick {Tick}][BehaviorSnap] entityId : {entityId}, behaviorMasterId : {behaviorMasterId}";
    }
}
