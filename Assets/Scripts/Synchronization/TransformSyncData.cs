using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

[Serializable]
public class TransformSyncData : ISyncData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;

    public TransformSyncData(IEntity entity)
    {
        Set(entity);
    }

    public TransformSyncData Set(IEntity entity)
    {
        position = entity.Position;
        rotation = entity.Rotation;
        velocity = entity.Velocity;
        angularVelocity = entity.AngularVelocity;

        return this;
    }
}
