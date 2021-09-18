using UnityEngine;
using Entity;
using EntityMessage;
using GameFramework;

public class GameItemPhysicsController : LOPMonoEntityComponentBase
{
    public override void OnAttached(IEntity entity)
    {
        base.OnAttached(entity);

        Entity.MessageBroker.AddSubscriber<ModelTriggerEnter>(OnModelTriggerEnter);
    }

    public override void OnDetached()
    {
        base.OnDetached();

        Entity.MessageBroker.RemoveSubscriber<ModelTriggerEnter>(OnModelTriggerEnter);
    }

    private void OnModelTriggerEnter(ModelTriggerEnter message)
    {
        Character target = Entities.Get<Character>(message.targetEntityId);
        if (target == null || !target.IsAlive)
            return;

        LOP.Game.Current.EntityGetGameItem(target.EntityID, Entity.EntityID);
    }
}
