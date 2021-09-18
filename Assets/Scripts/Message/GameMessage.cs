using UnityEngine;

namespace GameMessage
{
    public struct EntityDestroy
    {
        public int entityId;

        public EntityDestroy(int entityId)
        {
            this.entityId = entityId;
        }
    }

    public struct EntityMove
    {
        public int entityId;

        public EntityMove(int entityId)
        {
            this.entityId = entityId;
        }
    }

    public struct EntityMoveCell
    {
        public int entityId;
        public Vector2Int pre;
        public Vector2Int cur;

        public EntityMoveCell(int entityId, Vector2Int pre, Vector2Int cur)
        {
            this.entityId = entityId;
            this.pre = pre;
            this.cur = cur;
        }
    }

    public struct EntityAddedToGrid
    {
        public int entityId;
        public Vector2Int cellPosition;

        public EntityAddedToGrid(int entityId, Vector2Int cellPosition)
        {
            this.entityId = entityId;
            this.cellPosition = cellPosition;
        }
    }

    public struct EntityRemovedFromGrid
    {
        public int entityId;
        public Vector2Int cellPosition;

        public EntityRemovedFromGrid(int entityId, Vector2Int cellPosition)
        {
            this.entityId = entityId;
            this.cellPosition = cellPosition;
        }
    }

    public struct EntityRegister
    {
        public int entityId;

        public EntityRegister(int entityId)
        {
            this.entityId = entityId;
        }
    }

    public struct EntityUnregister
    {
        public int entityId;

        public EntityUnregister(int entityId)
        {
            this.entityId = entityId;
        }
    }
}
