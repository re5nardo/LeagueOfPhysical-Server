using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

public class Grid : GameFramework.Grid
{
    public override void Add(int nEntityID, bool bPublish = true)
    {
        IEntity entity = Entities.Get(nEntityID);

        Vector2Int vec2CellPosition = GetCellPosition(entity.Position);

        if (!m_Cells.ContainsKey(vec2CellPosition))
        {
            m_Cells.Add(vec2CellPosition, new Cell(vec2CellPosition));
        }

        if (m_Cells[vec2CellPosition].Add(nEntityID))
        {
            m_EntityIDCellPosition[nEntityID] = vec2CellPosition;

            if (bPublish)
            {
                SceneMessageBroker.Publish(new GameMessage.EntityAddedToGrid(nEntityID, vec2CellPosition));
            }
        }
    }

    public override void Remove(int nEntityID, bool bPublish = true)
    {
        if (m_Cells[m_EntityIDCellPosition[nEntityID]].Remove(nEntityID))
        {
            Vector2Int vec2CellPosition = m_EntityIDCellPosition[nEntityID];

            m_EntityIDCellPosition.Remove(nEntityID);

            if (bPublish)
            {
                SceneMessageBroker.Publish(new GameMessage.EntityRemovedFromGrid(nEntityID, vec2CellPosition));
            }
        }
    }

    public override void Move(int nEntityID)
    {
        if (!m_EntityIDCellPosition.ContainsKey(nEntityID))
        {
            return;
        }

        Vector2Int pre = m_EntityIDCellPosition[nEntityID];

        IEntity entity = Entities.Get(nEntityID);

        Vector2Int cur = GetCellPosition(entity.Position);

        if (pre != cur)
        {
            Remove(nEntityID, false);

            Add(nEntityID, false);

            SceneMessageBroker.Publish(new GameMessage.EntityMoveCell(nEntityID, pre, cur));
        }
    }

    public override List<IEntity> GetEntities(Vector3 vec3Position, float fRadius, List<Predicate<IEntity>> conditions)
    {
        float sqrRadius = fRadius * fRadius;

        List<IEntity> entities = new List<IEntity>();

        foreach (Cell cell in GetCells(GetCellPosition(vec3Position), fRadius))
        {
            foreach (int nEntityID in cell.m_hashEntityID)
            {
                IEntity entity = Entities.Get(nEntityID);

                if (!GameFramework.Util.IsSatisfy(entity, conditions))
                    continue;

                if ((vec3Position - entity.Position).sqrMagnitude <= sqrRadius)
                {
                    entities.Add(entity);
                }
            }
        }

        return entities;
    }

    public override List<IEntity> GetEntities(Transform trTarget, float fFieldOfViewAngle, float fRadius, List<Predicate<IEntity>> conditions)
    {
        float sqrRadius = fRadius * fRadius;

        List<IEntity> entities = new List<IEntity>();

        foreach (Cell cell in GetCells(GetCellPosition(trTarget.position), fRadius))
        {
            foreach (int nEntityID in cell.m_hashEntityID)
            {
                IEntity entity = Entities.Get(nEntityID);

                if (!GameFramework.Util.IsSatisfy(entity, conditions))
                    continue;

                Vector3 direction = trTarget.position - entity.Position;
                float fAngle = Vector3.Angle(direction, trTarget.forward);

                if (fAngle <= fFieldOfViewAngle && direction.sqrMagnitude <= sqrRadius)
                {
                    entities.Add(entity);
                }
            }
        }

        return entities;
    }

    public override List<IEntity> GetEntities(Vector2Int vec2CellPos, List<Predicate<IEntity>> conditions)
    {
        List<IEntity> entities = new List<IEntity>();

        if (m_Cells.ContainsKey(vec2CellPos))
        {
            foreach (int nEntityID in m_Cells[vec2CellPos].m_hashEntityID)
            {
                IEntity entity = Entities.Get(nEntityID);

                if (!GameFramework.Util.IsSatisfy(entity, conditions))
                    continue;

                entities.Add(entity);
            }
        }

        return entities;
    }
}
