using UnityEngine;
using System.Collections.Generic;
using System;
using GameFramework;
using Entity;

public class LOPEntityComponentBase : EntityComponentBase
{
    public new LOPMonoEntityBase Entity => base.Entity as LOPMonoEntityBase;
    public bool IsValid => Entity != null;
    public bool Initialized { get; private set; }

    public void Initialize(EntityCreationData entityCreationData)
    {
        OnInitialize(entityCreationData);

        Initialized = true;
    }

    protected virtual void OnInitialize(EntityCreationData entityCreationData)
    {
    }
}
