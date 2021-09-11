using UnityEngine;

using System.Collections.Generic;
using System;
using GameFramework;
using Entity;

public class EntityBasicData : MonoEntityComponentBase
{
    new protected MonoEntityBase Entity = null;

    public override void OnAttached(IEntity entity)
    {
        base.OnAttached(entity);

        Entity = entity as MonoEntityBase;
    }

    public override void OnDetached()
    {
        base.OnDetached();

        Entity = null;
    }
}
