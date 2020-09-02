using UnityEngine;
using Entity;
using EntityCommand;
using GameFramework;

public class GameItemPhysicsController : MonoComponentBase
{
    public override void OnAttached(IEntity entity)
    {
        base.OnAttached(entity);

        CommandHandlerOn(typeof(ModelTriggerEnter), OnModelTriggerEnter);
    }

    public override void OnDetached()
    {
        base.OnDetached();

        CommandHandlerOff(typeof(ModelTriggerEnter));
    }

    private void OnModelTriggerEnter(ICommand command)
    {
        ModelTriggerEnter cmd = command as ModelTriggerEnter;

        Character target = Entities.Get<Character>(cmd.targetEntityID);
        if (target == null || !target.IsAlive)
            return;

        LOP.Game.Current.EntityGetGameItem(target.EntityID, Entity.EntityID);
    }
}
