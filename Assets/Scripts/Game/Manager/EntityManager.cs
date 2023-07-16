﻿using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using Entity;

public class EntityManager : GameFramework.EntityManager
{
    #region Singlton Pattern
    protected static EntityManager instance = null;
    public static EntityManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject goSingleton = new GameObject("EntityManager");

                instance = goSingleton.AddComponent<EntityManager>();
            }

            return instance;
        }
    }

    public static bool HasInstance()
    {
        return instance != null;
    }

    public static void Instantiate()
    {
        if (instance == null)
        {
            GameObject goSingleton = new GameObject(typeof(EntityManager).Name + "Singleton");

            instance = goSingleton.AddComponent<EntityManager>();
        }
    }
    #endregion

    private void Awake()
    {
        positionGrid = new Grid();
        positionGrid.SetGrid(LOP.Game.GRID_SIZE);

        SceneMessageBroker.AddSubscriber<GameMessage.EntityMove>(OnEntityMove);
        SceneMessageBroker.AddSubscriber<TickMessage.Tick>(OnTick);
    }

    private void OnDestroy()
    {
        positionGrid.Clear();
        positionGrid = null;

        SceneMessageBroker.RemoveSubscriber<GameMessage.EntityMove>(OnEntityMove);
        SceneMessageBroker.RemoveSubscriber<TickMessage.Tick>(OnTick);
    }

    public override void RegisterEntity(IEntity entity)
    {
        base.RegisterEntity(entity);

        SceneMessageBroker.Publish(new GameMessage.EntityRegister(entity.EntityId));
    }

    public override void UnregisterEntity(int entityId)
    {
        base.UnregisterEntity(entityId);

        SceneMessageBroker.Publish(new GameMessage.EntityUnregister(entityId));
    }

    #region Message Handler
    private void OnEntityMove(GameMessage.EntityMove message)
    {
        positionGrid.Move(message.entityId);
    }
    #endregion

    public List<IEntity> GetEntities(Vector3 vec3Position, float fRadius, EntityRole entityRoleFlag, HashSet<int> hashExceptID = null)
    {
        return GetEntities(vec3Position, fRadius, new List<System.Predicate<IEntity>>
        {
            entity =>
            {
                return ((entity as LOPMonoEntityBase).EntityRole & entityRoleFlag) != 0;
            },
            entity =>
            {
                return !(hashExceptID != null && hashExceptID.Contains(entity.EntityId));
            }
        });
    }

    public List<IEntity> GetEntities(Transform trTarget, float fFieldOfViewAngle, float fRadius, EntityRole entityRoleFlag, HashSet<int> hashExceptID = null)
    {
        return GetEntities(trTarget, fFieldOfViewAngle, fRadius, new List<System.Predicate<IEntity>>
        {
            entity =>
            {
                return ((entity as LOPMonoEntityBase).EntityRole & entityRoleFlag) != 0;
            },
            entity =>
            {
                return !(hashExceptID != null && hashExceptID.Contains(entity.EntityId));
            }
        });
    }

    private void OnTick(TickMessage.Tick message)
    {
        //  sort
        //  ...

        GetAllEntities<LOPMonoEntityBase>()?.ForEach(entity =>
        {
            if (entity.IsValid)
            {
                entity.OnTick(message.tick);
            }
        });
    }
}
