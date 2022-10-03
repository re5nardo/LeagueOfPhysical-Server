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
        public const byte SC_Synchronization = 9;
        public const byte SC_GameEnd = 11;
        public const byte SC_OwnerChanged = 12;
        public const byte SC_SyncController = 13;
        public const byte SC_SubGameReadyNotice = 14;
        public const byte SC_PlayerEntity = 15;

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
        public string serverId;
        public int tick;
        public int entityId;
        public Vector3 position;
        public Vector3 rotation;
        public List<SyncControllerData> syncControllerDataList = new List<SyncControllerData>();
        public List<SyncDataEntry> syncDataEntries = new List<SyncDataEntry>();

        public byte GetMessageId()
        {
            return MessageIds.SC_EnterRoom;
        }

        public void Clear()
        {
            serverId = default;
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
        public Dictionary<int, double> dicSkillInfo = new Dictionary<int, double>();

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
    public class SC_GameEnd : IMirrorMessage
    {
        public List<int> listWinnerEntityId = new List<int>();
        public List<int> listLoserEntityId = new List<int>();
        public List<RankingData> listRankingData = new List<RankingData>();

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

    [Serializable]
    public class SC_SubGameReadyNotice : IMirrorMessage
    {
        public double timeBeforeStart;

        public byte GetMessageId()
        {
            return MessageIds.SC_SubGameReadyNotice;
        }

        public void Clear()
        {
            timeBeforeStart = default;
        }
    }

    [Serializable]
    public class SC_PlayerEntity : IMirrorMessage
    {
        public int playerEntityId;

        public byte GetMessageId()
        {
            return MessageIds.SC_PlayerEntity;
        }

        public void Clear()
        {
            playerEntityId = default;
        }
    }
    #endregion

    #region Protocols (Client to Server)
    [Serializable]
    public class CS_NotifyMoveInputData : IMirrorMessage
    {
        public PlayerMoveInput playerMoveInput;

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
