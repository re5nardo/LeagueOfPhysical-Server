using UnityEngine;
using Entity;
using EntityCommand;
using GameFramework;

public class GameItemPhysicsController : LOPMonoEntityComponentBase
{
    public override void OnAttached(IEntity entity)
    {
        base.OnAttached(entity);

        AddCommandHandler(typeof(ModelTriggerEnter), OnModelTriggerEnter);
    }

    public override void OnDetached()
    {
        base.OnDetached();

        RemoveCommandHandler(typeof(ModelTriggerEnter), OnModelTriggerEnter);
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
