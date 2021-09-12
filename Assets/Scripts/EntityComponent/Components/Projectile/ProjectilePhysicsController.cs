using UnityEngine;
using Entity;
using EntityCommand;
using GameFramework;
using System.Collections.Generic;

public class ProjectilePhysicsController : MonoEntityComponentBase
{
    private HashSet<int> attackList = new HashSet<int>();

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

        int projectorId = Entity.GetEntityComponent<ProjectileBasicData>().ProjectorId;

        IEntity target = Entities.Get(cmd.targetEntityID);
        if (target == null)
        {
            //  Already destroyed
            return;
        }
        else if (target is Character)
        {
            if (target.EntityID == projectorId || !(target as Character).IsAlive)
            {
                return;
            }
        }
        else if (target is GameItem)
        {
            if (!(target as GameItem).IsAlive)
            {
                return;
            }
        }
        else if (target is Projectile)
        {
            return;
        }

        if (!attackList.Contains(target.EntityID))
        {
            LOP.Game.Current.AttackEntity(projectorId, target.EntityID);

            LOP.Game.Current.DestroyEntity(Entity.EntityID);

            attackList.Add(target.EntityID);
        }
    }
}
