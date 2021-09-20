using UnityEngine;
using System.Collections.Generic;
using System;
using Entity;
using GameFramework;

public class LOPMonoEntityComponentBase : MonoEntityComponentBase
{
    public new LOPMonoEntityBase Entity { get; private set; }
    public bool IsValid => Entity != null;
    public bool Initialized { get; private set; }

    public override void OnAttached(IEntity entity)
    {
        base.OnAttached(entity);

        Entity = entity as LOPMonoEntityBase;
    }

    public override void OnDetached()
    {
        base.OnDetached();

        Entity = null;
    }

    public void Initialize(EntityCreationData entityCreationData)
    {
        OnInitialize(entityCreationData);

        Initialized = true;
    }

    protected virtual void OnInitialize(EntityCreationData entityCreationData)
    {
    }
}
