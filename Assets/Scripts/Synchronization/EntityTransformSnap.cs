using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;
using Behavior;

[Serializable]
public class EntityTransformSnap : ISnap
{
    public int Tick { get; set; }
    public int entityId;

    public float GameTime => Tick * Game.Current.TickInterval;

    public Vector3 position;
    public Vector3 rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;

    public EntityTransformSnap() { }

    public EntityTransformSnap(IEntity entity)
    {
        Tick = Game.Current.CurrentTick;
        entityId = entity.EntityID;
        position = entity.Position;
        rotation = entity.Rotation;
        velocity = entity.Velocity;
        angularVelocity = entity.AngularVelocity;
    }

    public ISnap Set(IEntity entity)
    {
        Tick = Game.Current.CurrentTick;
        entityId = entity.EntityID;
        position = entity.Position;
        rotation = entity.Rotation;
        velocity = entity.Velocity;
        angularVelocity = entity.AngularVelocity;

        return this;
    }

    public ISnap Clone()
    {
        EntityTransformSnap clone = new EntityTransformSnap();

        clone.Tick = Tick;
        clone.entityId = entityId;
        clone.position = position;
        clone.rotation = rotation;
        clone.velocity = velocity;
        clone.angularVelocity = angularVelocity;

        return clone;
    }

    public override string ToString()
    {
        return $"[Tick {Tick}][EntityTransformSnap] entityId : {entityId}, position : {position.ToString()}, " +
            $"rotation : {rotation.ToString()}, velocity : {velocity.ToString()}, angularVelocity : {angularVelocity.ToString()}";
    }
}
