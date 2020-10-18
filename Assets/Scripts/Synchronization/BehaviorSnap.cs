using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

[Serializable]
public class BehaviorSnap : ISnap
{
    public int Tick { get; set; }
    public string Id { get; set; }

    public BehaviorSnap() { }

    public BehaviorSnap(IEntity entity)
    {
        Tick = Game.Current.CurrentTick;
        Id = entity.EntityID.ToString();
    }

    public bool EqualsCore(ISnap snap)
    {
        BehaviorSnap other = snap as BehaviorSnap;

        if (other == null) return false;
        if (other.Id != Id) return false;

        return true;
    }

    public bool EqualsValue(ISnap snap)
    {
        BehaviorSnap other = snap as BehaviorSnap;

        if (other == null) return false;
        if (other.Id != Id) return false;

        return true;
    }

    public ISnap Set(ISynchronizable synchronizable)
    {
        var behaviorBase = synchronizable as Behavior.BehaviorBase;

        Tick = Game.Current.CurrentTick;
        Id = behaviorBase.Entity.EntityID.ToString();
  
        return this;
    }

    public ISnap Clone()
    {
        BehaviorSnap clone = new BehaviorSnap();

        clone.Tick = Tick;
        clone.Id = Id;
 
        return clone;
    }

    public override string ToString()
    {
        return $"[Tick {Tick}][BehaviorSnap] Id : {Id}";
    }
}
