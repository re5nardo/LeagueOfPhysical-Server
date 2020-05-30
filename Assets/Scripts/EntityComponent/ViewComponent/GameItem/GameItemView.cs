using UnityEngine;
using System.Collections.Generic;
using System;
using EntityCommand;
using GameFramework;

public class GameItemView : EntityBasicView
{
	#region Model Collision Handler
	protected override void OnModelTriggerEnterHandler(Collider me, Collider other)
	{
		EntityIDTag entityIDTag = other.GetComponent<EntityIDTag>();
		if (entityIDTag == null)
		{
			return;
		}

		Entity.SendCommandToAll(new ModelTriggerEnter(entityIDTag.GetEntityID()));
	}
	#endregion
}
