using UnityEngine;
using System.Collections.Generic;
using System;
using Entity;
using GameFramework;

public class LOPMonoEntityComponentBase : MonoEntityComponentBase
{
    public new LOPMonoEntityBase Entity { get; private set; }

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

    public virtual void Initialize(EntityCreationData entityCreationData)
    {
    }
}
