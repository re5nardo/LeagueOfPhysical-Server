using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCreationData
{
    public int entityId = -1;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;
    public EntityType entityType;
    public EntityRole entityRole;
    public string ownerId;

    public virtual void Clear()
    {
        entityId = -1;
        position = default;
        rotation = default;
        velocity = default;
        angularVelocity = default;
        entityType = default;
        entityRole = default;
        ownerId = default;
    }
}

public class CharacterCreationData : EntityCreationData
{
    public int masterDataId = -1;
    public string modelId;
    public FirstStatus firstStatus;
    public SecondStatus secondStatus;

    public override void Clear()
    {
        base.Clear();

        masterDataId = -1;
        modelId = default;
        firstStatus = default;
        secondStatus = default;
    }
}

public class GameItemCreationData : EntityCreationData
{
    public int masterDataId = -1;
    public string modelId;
    public float lifespan = -1;

    public override void Clear()
    {
        base.Clear();

        masterDataId = -1;
        modelId = default;
        lifespan = -1;
    }
}

public class ProjectileCreationData : EntityCreationData
{
    public int masterDataId = -1;
    public string modelId;
    public int projectorId = -1;
    public float lifespan = -1;
    public float movementSpeed;

    public override void Clear()
    {
        base.Clear();

        masterDataId = -1;
        modelId = default;
        projectorId = -1;
        lifespan = -1;
        movementSpeed = default;
    }
}
