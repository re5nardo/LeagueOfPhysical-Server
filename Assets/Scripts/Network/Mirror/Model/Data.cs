using UnityEngine;
using System;

namespace NetworkModel.Mirror
{
    #region EntitySnap
    [Serializable]
    public class EntitySnap
    {
        public int entityId = -1;
        public EntityType entityType;
        public EntityRole entityRole;
        public string ownerId;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 velocity;
        public Vector3 angularVelocity;

        public EntitySnap() { }
    }

    [Serializable]
    public class CharacterSnap : EntitySnap
    {
        public int masterDataId = -1;
        public string modelId;
        public FirstStatus firstStatus;
        public SecondStatus secondStatus;

        public CharacterSnap() { }
    }

    [Serializable]
    public class ProjectileSnap : EntitySnap
    {
        public int masterDataId = -1;
        public string modelId;
        public float movementSpeed;

        public ProjectileSnap() { }
    }

    [Serializable]
    public class GameItemSnap : EntitySnap
    {
        public int masterDataId = -1;
        public string modelId;
        public int HP;
        public int maximumHP;

        public GameItemSnap() { }
    }
    #endregion

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
