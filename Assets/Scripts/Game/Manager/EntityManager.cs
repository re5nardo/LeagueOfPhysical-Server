using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;
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
    #endregion

    private void Awake()
    {
        m_PositionGrid = new Grid();

        m_PositionGrid.SetGrid(10);

        GamePubSubService.AddSubscriber(GameMessageKey.EntityMove, OnEntityMove);
    }

    private void OnDestroy()
    {
        GamePubSubService.RemoveSubscriber(GameMessageKey.EntityMove, OnEntityMove);
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
}
