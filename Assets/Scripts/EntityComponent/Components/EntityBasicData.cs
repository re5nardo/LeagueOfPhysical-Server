using UnityEngine;

using System.Collections.Generic;
using System;
using GameFramework;
using Entity;

public class EntityBasicData : MonoEntityComponentBase
{
    new protected LOPMonoEntityBase Entity = null;

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
}
