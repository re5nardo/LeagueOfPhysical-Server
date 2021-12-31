using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

//  IPoolable 때문에, 매개변수가 없는 기본 생성자 필요 & nullable 한 녀석들은 조심해서 사용 (ex. List, Dictionary 등등..)
namespace NetworkModel.Mirror
{
    public class MessageIds
    {
        public const byte SC_EnterRoom = 1;
        public const byte SC_ProcessInputData = 2;
        public const byte SC_EntitySkillInfo = 3;
        public const byte SC_EmotionExpression = 4;
        public const byte SC_EntityAppear = 5;
        public const byte SC_EntityDisAppear = 6;
        public const byte SC_GameEvents = 7;
        public const byte SC_SyncTick = 8;
        public const byte SC_Synchronization = 9;
        public const byte SC_GameState = 10;
        public const byte SC_GameEnd = 11;
        public const byte SC_OwnerChanged = 12;
        public const byte SC_SyncController = 13;

        public const byte CS_NotifyMoveInputData = 112;
        public const byte CS_NotifySkillInputData = 113;
        public const byte CS_NotifyJumpInputData = 114;
        public const byte CS_RequestEmotionExpression = 115;
        public const byte CS_GamePreparation = 116;
        public const byte CS_SubGamePreparation = 117;
        public const byte CS_Synchronization = 118;
        public const byte CS_SyncController = 119;
    }

    #region Protocols (Server to Client)
    [Serializable]
    public class SC_EnterRoom : IMirrorMessage
    {
        public int tick;
        public int entityId;
        public Vector3 position;
        public Vector3 rotation;
        public List<SyncControllerData> syncControllerDataList = new List<SyncControllerData>();
        public List<SyncDataEntry> syncDataEntries = new List<SyncDataEntry>();

        public SC_EnterRoom() { }

        public byte GetMessageId()
        {
            return MessageIds.SC_EnterRoom;
        }

        public void Clear()
        {
            tick = default;
            entityId = default;
            position = default;
            rotation = default;
            syncControllerDataList.Clear();
            syncDataEntries.Clear();
        }
    }

    [Serializable]
    public class SC_ProcessInputData : IMirrorMessage
    {
        public int tick;
        public string type;
        public long sequence;

        public SC_ProcessInputData() { }

        public byte GetMessageId()
        {
            return MessageIds.SC_ProcessInputData;
        }

        public void Clear()
        {
            tick = default;
            type = default;
            sequence = default;
        }
    }

    [Serializable]
    public class SC_EntitySkillInfo : IMirrorMessage
    {
        public int entityId;
        public Dictionary<int, float> dicSkillInfo = new Dictionary<int, float>();

        public SC_EntitySkillInfo() { }

        public byte GetMessageId()
        {
            return MessageIds.SC_EntitySkillInfo;
        }

        public void Clear()
        {
            entityId = default;
            dicSkillInfo.Clear();
        }
    }

    [Serializable]
    public class SC_EmotionExpression : IMirrorMessage
    {
        public int entityId;
        public int emotionExpressionId;

        public SC_EmotionExpression() { }

        public byte GetMessageId()
        {
            return MessageIds.SC_EmotionExpression;
        }

        public void Clear()
        {
            entityId = default;
            emotionExpressionId = default;
        }
    }

    [Serializable]
    public class SC_EntityAppear : IMirrorMessage
    {
        public int tick;
        public List<EntitySnap> listEntitySnap = new List<EntitySnap>();

        public SC_EntityAppear() { }

        public byte GetMessageId()
        {
            return MessageIds.SC_EntityAppear;
        }

        public void Clear()
        {
            tick = default;
            listEntitySnap.Clear();
        }
    }

    [Serializable]
    public class SC_EntityDisAppear : IMirrorMessage
    {
        public List<int> listEntityId = new List<int>();

        public SC_EntityDisAppear() { }

        public byte GetMessageId()
        {
            return MessageIds.SC_EntityDisAppear;
        }

        public void Clear()
        {
            listEntityId.Clear();
        }
    }

    [Serializable]
    public class SC_GameEvents : IMirrorMessage
    {
        public List<IGameEvent> listGameEvent = new List<IGameEvent>();

        public SC_GameEvents() { }

        public SC_GameEvents(List<IGameEvent> listGameEvent)
        {
            this.listGameEvent = listGameEvent;
        }

        public byte GetMessageId()
        {
            return MessageIds.SC_GameEvents;
        }

        public void Clear()
        {
            listGameEvent.Clear();
        }
    }

    [Serializable]
    public class SC_SyncTick : IMirrorMessage
    {
        public int tick;

        public SC_SyncTick() { }

        public SC_SyncTick(int tick)
        {
            this.tick = tick;
        }

        public byte GetMessageId()
        {
            return MessageIds.SC_SyncTick;
        }

        public void Clear()
        {
            tick = default;
        }
    }

    [Serializable]
    public class SC_Synchronization : IMirrorMessage
    {
        public SyncDataEntry syncDataEntry;

        public byte GetMessageId()
        {
            return MessageIds.SC_Synchronization;
        }

        public void Clear()
        {
            syncDataEntry = default;
        }
    }

    [Serializable]
    public class SC_GameState : IMirrorMessage
    {
        public string gameState;
        public GameStateData gameStateData;

        public SC_GameState() { }

        public byte GetMessageId()
        {
            return MessageIds.SC_GameState;
        }

        public void Clear()
        {
            gameState = default;
            gameStateData = default;
        }
    }

    [Serializable]
    public class SC_GameEnd : IMirrorMessage
    {
        public List<int> listWinnerEntityId = new List<int>();
        public List<int> listLoserEntityId = new List<int>();
        public List<RankingData> listRankingData = new List<RankingData>();

        public SC_GameEnd() { }

        public byte GetMessageId()
        {
            return MessageIds.SC_GameEnd;
        }

        public void Clear()
        {
            listWinnerEntityId.Clear();
            listLoserEntityId.Clear();
            listRankingData.Clear();
        }
    }

    [Serializable]
    public class SC_OwnerChanged : IMirrorMessage
    {
        public int entityId;
        public string ownerId;

        public SC_OwnerChanged() { }

        public byte GetMessageId()
        {
            return MessageIds.SC_OwnerChanged;
        }

        public void Clear()
        {
            entityId = default;
            ownerId = default;
        }
    }

    [Serializable]
    public class SC_SyncController : IMirrorMessage
    {
        public SyncControllerData syncControllerData;

        public byte GetMessageId()
        {
            return MessageIds.SC_SyncController;
        }

        public void Clear()
        {
            syncControllerData = default;
        }
    }
    #endregion

    #region Protocols (Client to Server)
    [Serializable]
    public class CS_NotifyMoveInputData : IMirrorMessage
    {
        public PlayerMoveInput playerMoveInput;

        public CS_NotifyMoveInputData() { }

        public byte GetMessageId()
        {
            return MessageIds.CS_NotifyMoveInputData;
        }

        public void Clear()
        {
            playerMoveInput = default;
        }
    }

    [Serializable]
    public class CS_NotifySkillInputData : IMirrorMessage
    {
        public SkillInputData skillInputData;

        public CS_NotifySkillInputData() { }

        public byte GetMessageId()
        {
            return MessageIds.CS_NotifySkillInputData;
        }

        public void Clear()
        {
            skillInputData = default;
        }
    }

    [Serializable]
    public class CS_NotifyJumpInputData : IMirrorMessage
    {
        public JumpInputData jumpInputData;

        public CS_NotifyJumpInputData() { }

        public byte GetMessageId()
        {
            return MessageIds.CS_NotifyJumpInputData;
        }

        public void Clear()
        {
            jumpInputData = default;
        }
    }

    [Serializable]
    public class CS_RequestEmotionExpression : IMirrorMessage
    {
        public int entityId;
        public int emotionExpressionId;

        public CS_RequestEmotionExpression() { }

        public CS_RequestEmotionExpression(int entityId, int emotionExpressionId)
        {
            this.entityId = entityId;
            this.emotionExpressionId = emotionExpressionId;
        }

        public byte GetMessageId()
        {
            return MessageIds.CS_RequestEmotionExpression;
        }

        public void Clear()
        {
            entityId = default;
            emotionExpressionId = default;
        }
    }

    [Serializable]
    public class CS_GamePreparation : IMirrorMessage
    {
        public int entityId;
        public float preparation;

        public CS_GamePreparation() { }

        public CS_GamePreparation(int entityId, int preparation)
        {
            this.entityId = entityId;
            this.preparation = preparation;
        }

        public byte GetMessageId()
        {
            return MessageIds.CS_GamePreparation;
        }

        public void Clear()
        {
            entityId = default;
            preparation = default;
        }
    }

    [Serializable]
    public class CS_SubGamePreparation : IMirrorMessage
    {
        public int entityId;
        public float preparation;

        public CS_SubGamePreparation() { }

        public CS_SubGamePreparation(int entityId, int preparation)
        {
            this.entityId = entityId;
            this.preparation = preparation;
        }

        public byte GetMessageId()
        {
            return MessageIds.CS_SubGamePreparation;
        }

        public void Clear()
        {
            entityId = default;
            preparation = default;
        }
    }

    [Serializable]
    public class CS_Synchronization : IMirrorMessage
    {
        public SyncDataEntry syncDataEntry;

        public byte GetMessageId()
        {
            return MessageIds.CS_Synchronization;
        }

        public void Clear()
        {
            syncDataEntry = default;
        }
    }

    [Serializable]
    public class CS_SyncController : IMirrorMessage
    {
        public SyncControllerData syncControllerData;

        public byte GetMessageId()
        {
            return MessageIds.CS_SyncController;
        }

        public void Clear()
        {
            syncControllerData = default;
        }
    }
    #endregion
}
