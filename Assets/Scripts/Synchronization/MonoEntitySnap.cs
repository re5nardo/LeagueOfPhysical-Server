using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;
using Entity;

[Serializable]
public class MonoEntitySnap : ISnap
{
    public int Tick { get; set; }
    public int entityId;

    public List<ISnap> snaps = new List<ISnap>();

    public MonoEntitySnap() { }
    public MonoEntitySnap(MonoEntityBase monoEntityBase)
    {
        Tick = Game.Current.CurrentTick;
        entityId = monoEntityBase.EntityID;
    }

    public bool EqualsCore(ISnap snap)
    {
        MonoEntitySnap other = snap as MonoEntitySnap;

        if (other == null) return false;
        if (other.entityId != entityId) return false;

        return true;
    }

    public bool EqualsValue(ISnap snap)
    {
        MonoEntitySnap other = snap as MonoEntitySnap;

        if (other == null) return false;
        if (other.entityId != entityId) return false;

        return true;
    }

    public ISnap Set(ISynchronizable synchronizable)
    {
        var monoEntitySynchronization = synchronizable as MonoEntitySynchronization;

        Tick = Game.Current.CurrentTick;
        entityId = monoEntitySynchronization.Entity.EntityID;

        return this;
    }

    public ISnap Clone()
    {
        MonoEntitySnap clone = new MonoEntitySnap();

        clone.Tick = Tick;
        clone.entityId = entityId;
        snaps.ForEach(snap => clone.snaps.Add(snap.Clone()));

        return clone;
    }

    public override string ToString()
    {
        return $"[Tick {Tick}][MonoEntitySnap] entityId : {entityId}";
    }
}
