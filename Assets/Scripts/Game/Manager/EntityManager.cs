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

        positionGrid.SetGrid(10);

        GamePubSubService.AddSubscriber(GameMessageKey.EntityMove, OnEntityMove);

        TickPubSubService.AddSubscriber("Tick", OnTick);
    }

    private void OnDestroy()
    {
        GamePubSubService.RemoveSubscriber(GameMessageKey.EntityMove, OnEntityMove);

        TickPubSubService.RemoveSubscriber("Tick", OnTick);
    }

    public override void Clear()
    {
        base.Clear();

        positionGrid = null;

        GamePubSubService.RemoveSubscriber(GameMessageKey.EntityMove, OnEntityMove);
    }

    public override void RegisterEntity(IEntity entity)
    {
        base.RegisterEntity(entity);

        GamePubSubService.Publish(GameMessageKey.EntityRegister, new object[] { entity.EntityID });
    }

    public override void UnregisterEntity(int nEntityID)
    {
        base.UnregisterEntity(nEntityID);

        GamePubSubService.Publish(GameMessageKey.EntityUnregister, new object[] { nEntityID });
    }

    #region Message Handler
    private void OnEntityMove(object[] param)
    {
        int nEntityID = (int)param[0];

        positionGrid.Move(nEntityID);
    }
    #endregion

    public List<IEntity> GetEntities(Vector3 vec3Position, float fRadius, EntityRole entityRoleFlag, HashSet<int> hashExceptID = null)
    {
        return GetEntities(vec3Position, fRadius, new List<System.Predicate<IEntity>>
        {
            entity =>
            {
                return ((entity as LOPEntityBase).EntityRole & entityRoleFlag) != 0;
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
                return ((entity as LOPEntityBase).EntityRole & entityRoleFlag) != 0;
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

        GetAllEntities<LOPEntityBase>()?.ForEach(entity =>
        {
            if (entity.IsValid)
            {
                entity.OnTick(tick);
            }
        });
    }
}
