using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

[Serializable]
public class EntityTransformSnap : ISnap
{
    public int Tick { get; set; }
    public string Id { get; set; }

    public SerializableVector3 position;
    public SerializableVector3 rotation;
    public SerializableVector3 velocity;
    public SerializableVector3 angularVelocity;

    public EntityTransformSnap() { }

    public EntityTransformSnap(IEntity entity)
    {
        Tick = Game.Current.CurrentTick;
        Id = entity.EntityID.ToString();
        position = entity.Position;
        rotation = entity.Rotation;
        velocity = entity.Velocity;
        angularVelocity = entity.AngularVelocity;
    }

    public bool EqualsCore(ISnap snap)
    {
        EntityTransformSnap other = snap as EntityTransformSnap;

        if (other == null) return false;
        if (other.Id != Id) return false;
        if (other.velocity != velocity) return false;
        if (other.angularVelocity != angularVelocity) return false;

        return true;
    }

    public bool EqualsValue(ISnap snap)
    {
        EntityTransformSnap other = snap as EntityTransformSnap;

        if (other == null) return false;
        if (other.Id != Id) return false;
        if (other.position != position) return false;
        if (other.rotation != rotation) return false;
        if (other.velocity != velocity) return false;
        if (other.angularVelocity != angularVelocity) return false;

        return true;
    }

    public ISnap Set(ISynchronizable synchronizable)
    {
        var entityTransformSynchronization = synchronizable as EntityTransformSynchronization;

        Tick = Game.Current.CurrentTick;
        Id = entityTransformSynchronization.Entity.EntityID.ToString();
        position = entityTransformSynchronization.Entity.Position;
        rotation = entityTransformSynchronization.Entity.Rotation;
        velocity = entityTransformSynchronization.Entity.Velocity;
        angularVelocity = entityTransformSynchronization.Entity.AngularVelocity;

        return this;
    }

    public ISnap Clone()
    {
        EntityTransformSnap clone = new EntityTransformSnap();

        clone.Tick = Tick;
        clone.Id = Id;
        clone.position = position;
        clone.rotation = rotation;
        clone.velocity = velocity;
        clone.angularVelocity = angularVelocity;

        return clone;
    }

    public override string ToString()
    {
        return $"[Tick {Tick}][EntityTransformSnap] Id : {Id}, position : {position.ToString()}, rotation : {rotation.ToString()}, velocity : {velocity.ToString()}, angularVelocity : {angularVelocity.ToString()}";
    }
}
