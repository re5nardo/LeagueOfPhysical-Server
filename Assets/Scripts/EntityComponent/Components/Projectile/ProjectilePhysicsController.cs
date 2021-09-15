using UnityEngine;
using Entity;
using EntityMessage;
using GameFramework;
using System.Collections.Generic;
using UniRx;

public class ProjectilePhysicsController : LOPMonoEntityComponentBase
{
    private HashSet<int> attackList = new HashSet<int>();

    public override void OnAttached(IEntity entity)
    {
        base.OnAttached(entity);

        Entity.MessageBroker.Receive<ModelTriggerEnter>().Where(_ => IsValid).Subscribe(OnModelTriggerEnter);
    }

    private void OnModelTriggerEnter(ModelTriggerEnter message)
    {
        int projectorId = Entity.GetEntityComponent<ProjectileBasicData>().ProjectorId;

        IEntity target = Entities.Get(message.targetEntityID);
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
