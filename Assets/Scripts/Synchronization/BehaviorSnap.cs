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
    public string Id { get; set; }
    public string behaviorName;

    public BehaviorSnap() { }
    public BehaviorSnap(string id, string behaviorName)
    {
        Tick = Game.Current.CurrentTick;
        Id = id;
        this.behaviorName = behaviorName;
    }

    public virtual bool EqualsCore(ISnap snap)
    {
        BehaviorSnap other = snap as BehaviorSnap;

        if (other == null) return false;
        if (other.Id != Id) return false;
        if (other.behaviorName != behaviorName) return false;

        return true;
    }

    public virtual bool EqualsValue(ISnap snap)
    {
        BehaviorSnap other = snap as BehaviorSnap;

        if (other == null) return false;
        if (other.Id != Id) return false;
        if (other.behaviorName != behaviorName) return false;

        return true;
    }

    public virtual ISnap Set(ISynchronizable synchronizable)
    {
        var behaviorBase = synchronizable as BehaviorBase;

        Tick = Game.Current.CurrentTick;
        Id = behaviorBase.Entity.EntityID.ToString();
        behaviorName = behaviorBase.GetType().Name;

        return this;
    }

    public virtual ISnap Clone()
    {
        BehaviorSnap clone = new BehaviorSnap();

        clone.Tick = Tick;
        clone.Id = Id;
        clone.behaviorName = behaviorName;

        return clone;
    }

    public override string ToString()
    {
        return $"[Tick {Tick}][BehaviorSnap] Id : {Id}, behaviorName : {behaviorName}";
    }
}
