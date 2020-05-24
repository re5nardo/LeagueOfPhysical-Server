using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;
using Entity;

public class EntityManager : GameFramework.EntityManager, ISubscriber
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

    private Dictionary<Enum, Action<object[]>> m_dicMessageHandler = new Dictionary<Enum, Action<object[]>>();

    private void Awake()
    {
        m_PositionGrid = new Grid();

        m_PositionGrid.SetGrid(10);

        GamePubSubService.Instance.AddSubscriber(GameMessageKey.EntityMove, this);

        m_dicMessageHandler.Add(GameMessageKey.EntityMove, OnEntityMove);
    }

    private void OnDestroy()
    {
        if (GamePubSubService.IsInstantiated())
        {
            GamePubSubService.Instance.RemoveSubscriber(GameMessageKey.EntityMove, this);
        }

        m_dicMessageHandler.Clear();
    }

    public override void Clear()
    {
        base.Clear();

        m_PositionGrid = null;

        if (GamePubSubService.IsInstantiated())
        {
            GamePubSubService.Instance.RemoveSubscriber(GameMessageKey.EntityMove, this);
        }
      
        m_dicMessageHandler.Clear();
    }

    #region ISubscriber
    public void OnMessage(Enum key, params object[] param)
    {
        m_dicMessageHandler[key](param);
    }
    #endregion

    #region Message Handler
    private void OnEntityMove(params object[] param)
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
