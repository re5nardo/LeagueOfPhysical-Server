using System.Collections.Generic;
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

    public static bool IsInstantiated()
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
        m_PositionGrid = new Grid();

        m_PositionGrid.SetGrid(10);

        GamePubSubService.AddSubscriber(GameMessageKey.EntityMove, OnEntityMove);

        TickPubSubService.AddSubscriber("Tick", OnTick);
        TickPubSubService.AddSubscriber("BeforePhysicsSimulation", OnBeforePhysicsSimulation);
        TickPubSubService.AddSubscriber("AfterPhysicsSimulation", OnAfterPhysicsSimulation);
    }

    private void OnDestroy()
    {
        GamePubSubService.RemoveSubscriber(GameMessageKey.EntityMove, OnEntityMove);

        TickPubSubService.RemoveSubscriber("Tick", OnTick);
        TickPubSubService.RemoveSubscriber("BeforePhysicsSimulation", OnBeforePhysicsSimulation);
        TickPubSubService.RemoveSubscriber("AfterPhysicsSimulation", OnAfterPhysicsSimulation);
    }

    public override void Clear()
    {
        base.Clear();

        m_PositionGrid = null;

        GamePubSubService.RemoveSubscriber(GameMessageKey.EntityMove, OnEntityMove);
    }

    #region Message Handler
    private void OnEntityMove(object[] param)
    {
        int nEntityID = (int)param[0];

        m_PositionGrid.Move(nEntityID);
    }
    #endregion

    public List<IEntity> GetEntities(Vector3 vec3Position, float fRadius, EntityRole entityRoleFlag, HashSet<int> hashExceptID = null)
    {
        return GetEntities(vec3Position, fRadius, new List<System.Predicate<IEntity>>
        {
            entity =>
            {
                return ((entity as MonoEntityBase).EntityRole & entityRoleFlag) != 0;
            },
            entity =>
            {
                return !(hashExceptID != null && hashExceptID.Contains(entity.EntityID));
            }
        });
    }

    public List<IEntity> GetEntities(Transform trTarget, float fFieldOfViewAngle, float fRadius, EntityRole entityRoleFlag, HashSet<int> hashExceptID = null)
    {
        return GetEntities(trTarget, fFieldOfViewAngle, fRadius, new List<System.Predicate<IEntity>>
        {
            entity =>
            {
                return ((entity as MonoEntityBase).EntityRole & entityRoleFlag) != 0;
            },
            entity =>
            {
                return !(hashExceptID != null && hashExceptID.Contains(entity.EntityID));
            }
        });
    }

    private void OnTick(int tick)
    {
        //  sort
        //  ...

        GetAllEntities<MonoEntityBase>()?.ForEach(entity =>
        {
            if (entity.IsValid)
            {
                entity.OnTick(tick);
            }
        });
    }

    private void OnBeforePhysicsSimulation(int tick)
    {
        //  sort
        //  ...

        GetAllEntities<MonoEntityBase>()?.ForEach(entity =>
        {
            if (entity.IsValid)
            {
                entity.OnBeforePhysicsSimulation(tick);
            }
        });
    }

    private void OnAfterPhysicsSimulation(int tick)
    {
        //  sort
        //  ...

        GetAllEntities<MonoEntityBase>()?.ForEach(entity =>
        {
            if (entity.IsValid)
            {
                entity.OnAfterPhysicsSimulation(tick);
            }
        });
    }
}
