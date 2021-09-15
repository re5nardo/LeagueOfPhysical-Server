using UnityEngine;
using System.Collections.Generic;
using EntityMessage;

public class ProjectileView : EntityBasicView
{
	#region Model Collision Handler
	protected override void OnModelTriggerEnterHandler(Collider me, Collider other)
	{
		EntityIDTag entityIDTag = other.GetComponent<EntityIDTag>();
		if (entityIDTag == null)
		{
			return;
		}

		Entity.MessageBroker.Publish(new ModelTriggerEnter(entityIDTag.GetEntityID()));
	}
	#endregion
}
