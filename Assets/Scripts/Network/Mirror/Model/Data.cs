using UnityEngine;
using System;
using GameFramework;

namespace NetworkModel.Mirror
{
    [Serializable]
    public class EntityTransformInfo : IPoolable
    {
        public int tick = -1;
        public int entityId = -1;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 velocity;
        public Vector3 angularVelocity;

        public EntityTransformInfo() { }

        public EntityTransformInfo(int tick, IEntity entity)
        {
            this.tick = tick;
            entityId = entity.EntityID;
            position = entity.Position;
            rotation = entity.Rotation;
            velocity = entity.Velocity;
            angularVelocity = entity.AngularVelocity;
        }

        public void SetEntityTransformInfo(int tick, IEntity entity)
        {
            this.tick = tick;
            entityId = entity.EntityID;
            position = entity.Position;
            rotation = entity.Rotation;
            velocity = entity.Velocity;
            angularVelocity = entity.AngularVelocity;
        }

        public void Clear()
        {
            tick = -1;
            entityId = -1;
            position = default;
            rotation = default;
            velocity = default;
            angularVelocity = default;
        }
    }

    [Serializable]
    public class EntitySnapInfo
    {
        public int entityId = -1;
        public EntityType entityType;
        public EntityRole entityRole;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 velocity;
        public Vector3 angularVelocity;

        public EntitySnapInfo() { }
    }

    [Serializable]
    public class CharacterSnapInfo : EntitySnapInfo
    {
        public int masterDataId = -1;
        public string model;
        public FirstStatus firstStatus;
        public SecondStatus secondStatus;

        public CharacterSnapInfo() { }
    }

    [Serializable]
    public class ProjectileSnapInfo : EntitySnapInfo
    {
        public int masterDataId = -1;
        public string model;
        public float movementSpeed;

        public ProjectileSnapInfo() { }
    }

    [Serializable]
    public class GameItemSnapInfo : EntitySnapInfo
    {
        public int masterDataId = -1;
        public string model;
        public int HP;
        public int maximumHP;

        public GameItemSnapInfo() { }
    }

    [Serializable]
    public class SkillInputData
    {
        public int tick = -1;
        public int entityId = -1;
        public int skillId = -1;
        public Vector3 inputData = default;

        public SkillInputData() { }

        public SkillInputData(int tick, int entityId, int skillId, Vector3 inputData)
        {
            this.tick = tick;
            this.entityId = entityId;
            this.skillId = skillId;
            this.inputData = inputData;
        }
    }

    [Serializable]
    public class JumpInputData
    {
        public int tick = -1;
        public long sequence = -1;
        public int entityId = -1;

        public JumpInputData() { }

        public JumpInputData(int tick, long sequence, int entityId)
        {
            this.tick = tick;
            this.sequence = sequence;
            this.entityId = entityId;
        }
    }

    [Serializable]
    public class PlayerMoveInput
    {
        public int tick = -1;
        public long sequence = -1;
        public int entityId = -1;
        public Vector3 inputData = default;
        public InputType inputType = InputType.None;

        public enum InputType
        {
            None = 0,
            Press = 1,
            Hold = 2,
            Release = 3,
        }

        public PlayerMoveInput() { }

        public PlayerMoveInput(int tick = -1, long sequence = -1, int entityId = -1, Vector3 inputData = default, InputType inputType = InputType.None)
        {
            this.tick = tick;
            this.sequence = sequence;
            this.entityId = entityId;
            this.inputData = inputData;
            this.inputType = inputType;
        }
    }

    [Serializable]
    public class GameStateData
    {
    }

    [Serializable]
    public struct RankingData
    {
        public string entityId;
        public int ranking;

        public RankingData(string entityId, int ranking)
        {
            this.entityId = entityId;
            this.ranking = ranking;
        }
    }
}
