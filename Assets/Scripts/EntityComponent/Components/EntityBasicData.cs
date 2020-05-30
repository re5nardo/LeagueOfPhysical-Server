using UnityEngine;
using EntityCommand;
using System.Collections.Generic;
using System;
using GameFramework;
using Entity;

public class EntityBasicData : MonoComponentBase
{
	protected string m_strModel = "";

    new protected MonoEntityBase Entity = null;

    public override void Initialize(params object[] param)
	{
		base.Initialize(param);

		m_strModel = (string)param[0];

		Entity.SendCommandToViews(new ModelChanged(m_strModel));
	}

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

    public string ModelName { get { return m_strModel; } }
}
