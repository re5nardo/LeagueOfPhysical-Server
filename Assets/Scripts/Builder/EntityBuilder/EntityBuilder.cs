using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public abstract class EntityBuilder<Builder, Entity, Data> where Builder : EntityBuilder<Builder, Entity, Data> where Entity : class, IEntity where Data : EntityCreationData
{
    protected abstract Data entityCreationData { get; set; }

    public Builder SetEntityId(int entityId)
    {
        entityCreationData.entityId = entityId;
        return this as Builder;
    }

    public Builder SetPosition(Vector3 position)
    {
        entityCreationData.position = position;
        return this as Builder;
    }

    public Builder SetRotation(Vector3 rotation)
    {
        entityCreationData.rotation = rotation;
        return this as Builder;
    }

    public Builder SetVelocity(Vector3 velocity)
    {
        entityCreationData.velocity = velocity;
        return this as Builder;
    }

    public Builder SetAngularVelocity(Vector3 angularVelocity)
    {
        entityCreationData.angularVelocity = angularVelocity;
        return this as Builder;
    }

    public Builder SetEntityType(EntityType entityType)
    {
        entityCreationData.entityType = entityType;
        return this as Builder;
    }

    public Builder SetEntityRole(EntityRole entityRole)
    {
        entityCreationData.entityRole = entityRole;
        return this as Builder;
    }

    public Builder SetOwnerId(string ownerId)
    {
        entityCreationData.ownerId = ownerId;
        return this as Builder;
    }

    public abstract Entity Build();

    public void Clear()
    {
        entityCreationData.Clear();
    }
}
