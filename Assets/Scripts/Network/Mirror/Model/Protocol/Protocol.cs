using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;
using Mirror;

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

        public const byte CS_NotifyMoveInputData = 12;
        public const byte CS_NotifySkillInputData = 13;
        public const byte CS_NotifyJumpInputData = 14;
        public const byte CS_RequestEmotionExpression = 15;
        public const byte CS_GamePreparation = 16;
        public const byte CS_SubGamePreparation = 17;
    }

    #region Protocols (Server to Client)
    [Serializable]
    public class SC_EnterRoom : IMirrorMessage
    {
        public int Sender { get; set; }
        public int tick;
        public int entityId;
        public Vector3 position;
        public Vector3 rotation;

        public SC_EnterRoom() { }

        public byte GetMessageId()
        {
            return MessageIds.SC_EnterRoom;
        }

        public void Clear()
        {
            Sender = default;
            tick = default;
            entityId = default;
            position = default;
            rotation = default;
        }
    }

    [Serializable]
    public class SC_ProcessInputData : IMirrorMessage
    {
        public int Sender { get; set; }
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
            Sender = default;
            tick = default;
            type = default;
            sequence = default;
        }
    }

    [Serializable]
    public class SC_EntitySkillInfo : IMirrorMessage
    {
        public int Sender { get; set; }
        public int entityId;
        public Dictionary<int, float> dicSkillInfo = new Dictionary<int, float>();

        public SC_EntitySkillInfo() { }

        public byte GetMessageId()
        {
            return MessageIds.SC_EntitySkillInfo;
        }

        public void Clear()
        {
            Sender = default;
            entityId = default;
            dicSkillInfo.Clear();
        }
    }

    [Serializable]
    public class SC_EmotionExpression : IMirrorMessage
    {
        public int Sender { get; set; }
        public int entityId;
        public int emotionExpressionId;

        public SC_EmotionExpression() { }

        public byte GetMessageId()
        {
            return MessageIds.SC_EmotionExpression;
        }

        public void Clear()
        {
            Sender = default;
            entityId = default;
            emotionExpressionId = default;
        }
    }

    [Serializable]
    public class SC_EntityAppear : IMirrorMessage
    {
        public int Sender { get; set; }
        public int tick;
        public List<EntitySnapInfo> listEntitySnapInfo = new List<EntitySnapInfo>();

        public SC_EntityAppear() { }

        public byte GetMessageId()
        {
            return MessageIds.SC_EntityAppear;
        }

        public void Clear()
        {
            Sender = default;
            tick = default;
            listEntitySnapInfo.Clear();
        }
    }

    [Serializable]
    public class SC_EntityDisAppear : IMirrorMessage
    {
        public int Sender { get; set; }
        public List<int> listEntityId = new List<int>();

        public SC_EntityDisAppear() { }

        public byte GetMessageId()
        {
            return MessageIds.SC_EntityDisAppear;
        }

        public void Clear()
        {
            Sender = default;
            listEntityId.Clear();
        }
    }

    [Serializable]
    public class SC_GameEvents : IMirrorMessage
    {
        public int Sender { get; set; }
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
            Sender = default;
            listGameEvent.Clear();
        }
    }

    [Serializable]
    public class SC_SyncTick : IMirrorMessage
    {
        public int Sender { get; set; }
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
            Sender = default;
            tick = default;
        }
    }

    [Serializable]
    public class SC_Synchronization : IMirrorMessage
    {
        public int Sender { get; set; }
        public List<ISnap> listSnap = new List<ISnap>();

        public SC_Synchronization() { }

        public byte GetMessageId()
        {
            return MessageIds.SC_Synchronization;
        }

        public void Clear()
        {
            Sender = default;
            listSnap.Clear();
        }
    }

    [Serializable]
    public class SC_GameState : IMirrorMessage
    {
        public int Sender { get; set; }
        public string gameState;
        public GameStateData gameStateData;

        public SC_GameState() { }

        public byte GetMessageId()
        {
            return MessageIds.SC_GameState;
        }

        public void Clear()
        {
            Sender = default;
            gameState = default;
            gameStateData = default;
        }
    }

    [Serializable]
    public class SC_GameEnd : IMirrorMessage
    {
        public int Sender { get; set; }
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
            Sender = default;
            listWinnerEntityId.Clear();
            listLoserEntityId.Clear();
            listRankingData.Clear();
        }
    }
    #endregion

    #region Protocols (Client to Server)
    [Serializable]
    public class CS_NotifyMoveInputData : IMirrorMessage
    {
        public int Sender { get; set; }
        public PlayerMoveInput playerMoveInput;

        public CS_NotifyMoveInputData() { }

        public byte GetMessageId()
        {
            return MessageIds.CS_NotifyMoveInputData;
        }

        public void Clear()
        {
            Sender = default;
            playerMoveInput = default;
        }
    }

    [Serializable]
    public class CS_NotifySkillInputData : IMirrorMessage
    {
        public int Sender { get; set; }
        public SkillInputData skillInputData;

        public CS_NotifySkillInputData() { }

        public byte GetMessageId()
        {
            return MessageIds.CS_NotifySkillInputData;
        }

        public void Clear()
        {
            Sender = default;
            skillInputData = default;
        }
    }

    [Serializable]
    public class CS_NotifyJumpInputData : IMirrorMessage
    {
        public int Sender { get; set; }
        public JumpInputData jumpInputData;

        public CS_NotifyJumpInputData() { }

        public byte GetMessageId()
        {
            return MessageIds.CS_NotifyJumpInputData;
        }

        public void Clear()
        {
            Sender = default;
            jumpInputData = default;
        }
    }

    [Serializable]
    public class CS_RequestEmotionExpression : IMirrorMessage
    {
        public int Sender { get; set; }
        public int emotionExpressionId;

        public CS_RequestEmotionExpression() { }

        public CS_RequestEmotionExpression(int emotionExpressionId)
        {
            this.emotionExpressionId = emotionExpressionId;
        }

        public byte GetMessageId()
        {
            return MessageIds.CS_RequestEmotionExpression;
        }

        public void Clear()
        {
            Sender = default;
            emotionExpressionId = default;
        }
    }

    [Serializable]
    public class CS_GamePreparation : IMirrorMessage
    {
        public int Sender { get; set; }
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
            Sender = default;
            entityId = default;
            preparation = default;
        }
    }

    [Serializable]
    public class CS_SubGamePreparation : IMirrorMessage
    {
        public int Sender { get; set; }
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
            Sender = default;
            entityId = default;
            preparation = default;
        }
    }
    #endregion
}
