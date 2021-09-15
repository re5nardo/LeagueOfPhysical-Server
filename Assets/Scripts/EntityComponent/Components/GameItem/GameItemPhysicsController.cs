using UnityEngine;
using Entity;
using EntityMessage;
using GameFramework;
using UniRx;

public class GameItemPhysicsController : LOPMonoEntityComponentBase
{
    public override void OnAttached(IEntity entity)
    {
        base.OnAttached(entity);

        Entity.MessageBroker.Receive<ModelTriggerEnter>().Where(_ => IsValid).Subscribe(OnModelTriggerEnter);
    }

    private void OnModelTriggerEnter(ModelTriggerEnter message)
    {
        Character target = Entities.Get<Character>(message.targetEntityID);
        if (target == null || !target.IsAlive)
            return;

        LOP.Game.Current.EntityGetGameItem(target.EntityID, Entity.EntityID);
    }
}
