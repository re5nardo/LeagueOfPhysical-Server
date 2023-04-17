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

        if (!cells.ContainsKey(vec2CellPosition))
        {
            cells.Add(vec2CellPosition, new Cell(vec2CellPosition));
        }

        if (cells[vec2CellPosition].Add(nEntityID))
        {
            entityIDCellPosition[nEntityID] = vec2CellPosition;

            if (bPublish)
            {
                SceneMessageBroker.Publish(new GameMessage.EntityAddedToGrid(nEntityID, vec2CellPosition));
            }
        }
    }

    public override void Remove(int nEntityID, bool bPublish = true)
    {
        if (cells[entityIDCellPosition[nEntityID]].Remove(nEntityID))
        {
            Vector2Int vec2CellPosition = entityIDCellPosition[nEntityID];

            entityIDCellPosition.Remove(nEntityID);

            if (bPublish)
            {
                SceneMessageBroker.Publish(new GameMessage.EntityRemovedFromGrid(nEntityID, vec2CellPosition));
            }
        }
    }

    public override void Move(int nEntityID)
    {
        if (!entityIDCellPosition.ContainsKey(nEntityID))
        {
            return;
        }

        Vector2Int pre = entityIDCellPosition[nEntityID];

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
            foreach (int nEntityID in cell.hashEntityId)
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
            foreach (int nEntityID in cell.hashEntityId)
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

        if (cells.ContainsKey(vec2CellPos))
        {
            foreach (int nEntityID in cells[vec2CellPos].hashEntityId)
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
