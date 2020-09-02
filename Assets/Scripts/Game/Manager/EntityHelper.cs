using Entity;
using GameFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Entities
{
    public static IEntity Get(int entityId) 
    {
        return EntityManager.Instance.GetEntity(entityId);
    }

    public static T Get<T>(int entityId) where T : IEntity
    {
        return EntityManager.Instance.GetEntity<T>(entityId);
    }

    public static List<IEntity> Get(Vector3 vec3Position, float fRadius, List<Predicate<IEntity>> conditions)
    {
        return EntityManager.Instance.GetEntities(vec3Position, fRadius, conditions);
    }

    public static List<T> Get<T>(Vector3 vec3Position, float fRadius, List<Predicate<IEntity>> conditions) where T : IEntity
    {
        return EntityManager.Instance.GetEntities<T>(vec3Position, fRadius, conditions);
    }

    public static List<IEntity> Get(Transform trTarget, float fFieldOfViewAngle, float fRadius, List<System.Predicate<IEntity>> conditions)
    {
        return EntityManager.Instance.GetEntities(trTarget, fFieldOfViewAngle, fRadius, conditions);
    }

    public static List<T> Get<T>(Transform trTarget, float fFieldOfViewAngle, float fRadius, List<System.Predicate<IEntity>> conditions) where T : IEntity
    {
        return EntityManager.Instance.GetEntities<T>(trTarget, fFieldOfViewAngle, fRadius, conditions);
    }

    public static List<IEntity> Get(Vector3 vec3Position, float fRadius, EntityRole entityRoleFlag, HashSet<int> hashExceptID = null)
    {
        return EntityManager.Instance.GetEntities(vec3Position, fRadius, entityRoleFlag, hashExceptID);
    }

    public static List<IEntity> Get(Transform trTarget, float fFieldOfViewAngle, float fRadius, EntityRole entityRoleFlag, HashSet<int> hashExceptID = null)
    {
        return EntityManager.Instance.GetEntities(trTarget, fFieldOfViewAngle, fRadius, entityRoleFlag, hashExceptID);
    }

    #region All
    public static List<IEntity> All => EntityManager.Instance.GetAllEntities();

    public static List<T> GetAll<T>() where T : IEntity
    {
        return EntityManager.Instance.GetAllEntities<T>();
    }

    public static HashSet<int> AllIDs => EntityManager.Instance.GetAllEntityIDs();
    #endregion
}
