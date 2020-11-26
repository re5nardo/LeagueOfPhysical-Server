using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;
using Behavior;

[Serializable]
public class MoveSnap : BehaviorSnap
{
    public SerializableVector3 destination;

    public MoveSnap() { }
    public MoveSnap(BehaviorBase behavior) : base(behavior) { }

    public override bool EqualsCore(ISnap snap)
    {
        if (!base.EqualsCore(snap))
        {
            return false;
        }

        MoveSnap other = snap as MoveSnap;

        if (other == null) return false;
        if (other.destination != destination) return false;

        return true;
    }

    public override bool EqualsValue(ISnap snap)
    {
        if (!base.EqualsValue(snap))
        {
            return false;
        }

        MoveSnap other = snap as MoveSnap;

        if (other == null) return false;
        if (other.destination != destination) return false;

        return true;
    }

    public override ISnap Set(ISynchronizable synchronizable)
    {
        base.Set(synchronizable);

        var move = synchronizable as Move;
        destination = move.GetDestination();

        return this;
    }

    public override ISnap Clone()
    {
        MoveSnap clone = new MoveSnap();

        clone.Tick = Tick;
        clone.entityId = entityId;
        clone.behaviorMasterId = behaviorMasterId;
        clone.destination = destination;

        return clone;
    }

    public override string ToString()
    {
        return $"{base.ToString()}, destination : {destination}";
    }
}
