using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

//  0 ~ 255 (except reserved 16, 17, 22, 23) 
public class CustomSerializationCode
{
	public const byte SC_EnterRoom = 3;
	public const byte SC_NearEntityTransformInfos = 4;
	public const byte SC_EntitySkillInfo = 5;
	public const byte SC_PlayerMoveInputResponse = 6;
	public const byte SC_EmotionExpression = 7;
	public const byte SC_EntityAppear = 14;
	public const byte SC_EntityDisAppear = 15;
	public const byte SC_Ping = 19;
	public const byte SC_SelectableFirstStatusCount = 20;
	public const byte SC_CharacterStatusChange = 21;
	public const byte SC_SelectableAbilityInfo = 24;
    public const byte SC_GameEvents = 25;
    public const byte SC_SyncTick = 26;

	public const byte CS_NotifyMoveInputData = 130;
	public const byte CS_NotifyPlayerLookAtPosition = 131;
	public const byte CS_NotifySkillInputData = 132;
	public const byte CS_RequestEmotionExpression = 134;
	public const byte CS_Ping = 135;
	public const byte CS_FirstStatusSelection = 136;
	public const byte CS_AbilitySelection = 137;
}

//  0 ~ 199
public class PhotonEvent
{
	public const byte SC_EnterRoom = 0;
	public const byte SC_NearEntityTransformInfos = 1;
	public const byte SC_EntitySkillInfo = 2;
	public const byte SC_PlayerMoveInputResponse = 3;
	public const byte SC_EmotionExpression = 4;
	public const byte SC_EntityAppear = 11;
	public const byte SC_EntityDisAppear = 12;
	public const byte SC_Ping = 14;
	public const byte SC_SelectableFirstStatusCount = 15;
	public const byte SC_CharacterStatusChange = 16;
	public const byte SC_SelectableAbilityInfo = 17;
    public const byte SC_GameEvents = 18;
    public const byte SC_SyncTick = 19;

	public const byte CS_NotifyMoveInputData = 100;
	public const byte CS_NotifyPlayerLookAtPosition = 101;
	public const byte CS_NotifySkillInputData = 102;
	public const byte CS_RequestEmotionExpression = 104;
	public const byte CS_Ping = 105;
	public const byte CS_FirstStatusSelection = 106;
	public const byte CS_AbilitySelection = 107;
}

[Serializable]
public class EntityTransformInfo : IPoolable
{
    public EntityTransformInfo()
    {
    }

    public EntityTransformInfo(IEntity entity, float gameTime)
    {
        m_nEntityID = entity.EntityID;
        m_Position = entity.Position;
        m_Rotation = entity.Rotation;
        m_Velocity = entity.Velocity;
		m_AngularVelocity = entity.AngularVelocity;
		m_GameTime = gameTime;
    }

    public void SetEntityTransformInfo(IEntity entity, float gameTime)
    {
        m_nEntityID = entity.EntityID;
        m_Position = entity.Position;
        m_Rotation = entity.Rotation;
        m_Velocity = entity.Velocity;
		m_AngularVelocity = entity.AngularVelocity;
		m_GameTime = gameTime;
    }

    public void Clear()
    {
        m_nEntityID = -1;
        m_Position = default;
        m_Rotation = default;
        m_Velocity = default;
		m_AngularVelocity = default;
		m_GameTime = 0;
    }

    public int m_nEntityID = -1;
    public SerializableVector3 m_Position;
    public SerializableVector3 m_Rotation;
    public SerializableVector3 m_Velocity;
	public SerializableVector3 m_AngularVelocity;
	public float m_GameTime;
}

[Serializable]
public class EntitySnapInfo
{
	public int m_nEntityID = -1;
	public EntityType m_EntityType;
    public EntityRole m_EntityRole;
    public SerializableVector3 m_Position;
	public SerializableVector3 m_Rotation;
	public SerializableVector3 m_Velocity;
	public SerializableVector3 m_AngularVelocity;
}

[Serializable]
public class CharacterSnapInfo : EntitySnapInfo
{
	public int m_nMasterDataID = -1;
	public string m_strModel;
	public FirstStatus m_FirstStatus;
	public SecondStatus m_SecondStatus;
	public int m_nSelectableFirstStatusCount;
}

[Serializable]
public class ProjectileSnapInfo : EntitySnapInfo
{
	public int m_nMasterDataID = -1;
	public string m_strModel;
	public float m_fMovementSpeed;
}

[Serializable]
public class GameItemSnapInfo : EntitySnapInfo
{
	public int m_nMasterDataID = -1;
	public string m_strModel;
	public int m_nHP;
	public int m_nMaximumHP;
}

[Serializable]
public class SkillInputData
{
	public int m_nEntityID = -1;
	public int m_nSkillID = -1;
	public SerializableVector3 m_InputData = default;
	public float m_fGameTime = -1;

	public SkillInputData(int nEntityID, int nSkillID, SerializableVector3 inputData, float fGameTime)
	{
		m_nEntityID = nEntityID;
		m_nSkillID = nSkillID;
		m_InputData = inputData;
		m_fGameTime = fGameTime;
	}
}

[Serializable]
public struct SerializableVector3
{
	public float x;
	public float y;
	public float z;

	public SerializableVector3(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public SerializableVector3(Vector3 vec3)
	{
		x = vec3.x;
		y = vec3.y;
		z = vec3.z;
	}

	public static implicit operator SerializableVector3(Vector3 value)
	{
		return new SerializableVector3(value.x, value.y, value.z);
	}

	public static implicit operator Vector3(SerializableVector3 value)
	{
		return new Vector3(value.x, value.y, value.z);
	}

	public Vector3 ToVector3()
	{
		return new Vector3(x, y, z);
	}

    public override string ToString()
    {
        return ToVector3().ToString();
    }
}

[Serializable]
public class PlayerMoveInput
{
	public enum InputType
	{
		None = 0,
		Press = 1,
		Hold = 2,
		Release = 3,
	}

    public PlayerMoveInput()
    {
    }

    public PlayerMoveInput(int tick = -1, long sequence = -1, int entityID = -1, SerializableVector3 position = default, SerializableVector3 rotation = default, SerializableVector3 inputData = default, InputType inputType = InputType.None)
    {
        this.tick = tick;
        this.sequence = sequence;
        this.entityID = entityID;
        this.position = position;
        this.rotation = rotation;
        this.inputData = inputData;
        this.inputType = inputType;
    }

    public int tick = -1;
    public long sequence = -1;
    public int entityID = -1;
    public SerializableVector3 position = default;
    public SerializableVector3 rotation = default;
    public SerializableVector3 inputData = default;
    public InputType inputType = InputType.None;
}

[Serializable]
public class PlayerTransformInput
{
    public int tick = -1;
    public long sequence = -1;
    public int entityID = -1;
    public SerializableVector3 position = default;
    public SerializableVector3 rotation = default;
    public SerializableVector3 positionOffset = default;
    public SerializableVector3 rotationOffset = default;
}

#region Protocols (Server to Client)
[Serializable]
public class SC_EnterRoom : IPhotonEventMessage
{
    public int senderID { get; set; }
    public int m_nEntityID;
	public SerializableVector3 m_vec3Position;
	public SerializableVector3 m_vec3Rotation;
	public int m_nCurrentTick;

	public byte GetEventID()
	{
		return PhotonEvent.SC_EnterRoom;
	}
}

[Serializable]
public class SC_NearEntityTransformInfos : IPhotonEventMessage, IPoolable
{
    public int senderID { get; set; }
    public int tick = -1;
    public List<EntityTransformInfo> entityTransformInfos = new List<EntityTransformInfo>();

	public byte GetEventID()
	{
		return PhotonEvent.SC_NearEntityTransformInfos;
	}

	public void Clear()
	{
		foreach (var entityTransformInfo in entityTransformInfos)
		{
			ObjectPool.Instance.ReturnObject(entityTransformInfo);
		}
        entityTransformInfos.Clear();
	}
}

[Serializable]
public class SC_EntitySkillInfo : IPhotonEventMessage
{
    public int senderID { get; set; }
    public int m_nEntityID = -1;
	public Dictionary<int, float> m_dicSkillInfo = new Dictionary<int, float>();

	public byte GetEventID()
	{
		return PhotonEvent.SC_EntitySkillInfo;
	}
}

[Serializable]
public class SC_PlayerMoveInputResponse : IPhotonEventMessage
{
    public int senderID { get; set; }
    public int m_nTick = -1;
    public int m_nEntityID = -1;
	public SerializableVector3 m_Position;
	public SerializableVector3 m_Rotation;
	public long m_lLastProcessedSequence = -1;

	public byte GetEventID()
	{
		return PhotonEvent.SC_PlayerMoveInputResponse;
	}
}

[Serializable]
public class SC_EmotionExpression : IPhotonEventMessage
{
    public int senderID { get; set; }
    public int m_nEntityID;
	public int m_nEmotionExpressionID;

	public byte GetEventID()
	{
		return PhotonEvent.SC_EmotionExpression;
	}
}

[Serializable]
public class SC_EntityAppear : IPhotonEventMessage
{
    public int senderID { get; set; }
    public List<EntitySnapInfo> m_listEntitySnapInfo;
	public float m_fGameTime;

	public byte GetEventID()
	{
		return PhotonEvent.SC_EntityAppear;
	}
}

[Serializable]
public class SC_EntityDisAppear : IPhotonEventMessage
{
    public int senderID { get; set; }
    public List<int> m_listEntityID;

	public byte GetEventID()
	{
		return PhotonEvent.SC_EntityDisAppear;
	}
}

[Serializable]
public class SC_Ping : IPhotonEventMessage
{
    public int senderID { get; set; }
    public byte GetEventID()
	{
		return PhotonEvent.SC_Ping;
	}
}

[Serializable]
public class SC_SelectableFirstStatusCount : IPhotonEventMessage
{
    public int senderID { get; set; }
    public int m_nSelectableFirstStatusCount;

	public SC_SelectableFirstStatusCount(int nSelectableFirstStatusCount)
	{
		m_nSelectableFirstStatusCount = nSelectableFirstStatusCount;
	}

	public byte GetEventID()
	{
		return PhotonEvent.SC_SelectableFirstStatusCount;
	}
}

[Serializable]
public class SC_CharacterStatusChange : IPhotonEventMessage
{
    public int senderID { get; set; }
    public FirstStatus m_FirstStatus;
	public SecondStatus m_SecondStatus;

	public SC_CharacterStatusChange(FirstStatus firstStatus, SecondStatus secondStatus)
	{
		m_FirstStatus = firstStatus;
		m_SecondStatus = secondStatus;
	}

	public byte GetEventID()
	{
		return PhotonEvent.SC_CharacterStatusChange;
	}
}

[Serializable]
public class SC_SelectableAbilityInfo : IPhotonEventMessage
{
    public int senderID { get; set; }
    public List<int> m_SelectableAbilityIDs = new List<int>();

	public SC_SelectableAbilityInfo(List<int> selectableAbilityIDs)
	{
		m_SelectableAbilityIDs = selectableAbilityIDs;
	}

	public byte GetEventID()
	{
		return PhotonEvent.SC_SelectableAbilityInfo;
	}
}

[Serializable]
public class SC_GameEvents : IPhotonEventMessage
{
    public int senderID { get; set; }
    public List<IGameEvent> gameEvents = new List<IGameEvent>();

    public SC_GameEvents(List<IGameEvent> gameEvents)
    {
        this.gameEvents = gameEvents;
    }

    public byte GetEventID()
    {
        return PhotonEvent.SC_GameEvents;
    }
}

[Serializable]
public class SC_SyncTick : IPhotonEventMessage
{
    public int senderID { get; set; }
    public int tick;

    public SC_SyncTick(int tick)
    {
        this.tick = tick;
    }

    public byte GetEventID()
    {
        return PhotonEvent.SC_SyncTick;
    }
}
#endregion

#region Protocols (Client to Server)
[Serializable]
public class CS_NotifyMoveInputData : IPhotonEventMessage
{
    public int senderID { get; set; }
    public PlayerMoveInput m_PlayerMoveInput;

	public byte GetEventID()
	{
		return PhotonEvent.CS_NotifyMoveInputData;
	}
}

[Serializable]
public class CS_NotifyPlayerLookAtPosition : IPhotonEventMessage
{
    public int senderID { get; set; }
    public SerializableVector3 m_vec3Position;

	public byte GetEventID()
	{
		return PhotonEvent.CS_NotifyPlayerLookAtPosition;
	}
}

[Serializable]
public class CS_NotifySkillInputData : IPhotonEventMessage
{
    public int senderID { get; set; }
    public SkillInputData m_SkillInputData;

	public byte GetEventID()
	{
		return PhotonEvent.CS_NotifySkillInputData;
	}
}

[Serializable]
public class CS_RequestEmotionExpression : IPhotonEventMessage
{
    public int senderID { get; set; }
    public int m_nEmotionExpressionID;

	public CS_RequestEmotionExpression(int nEmotionExpressionID)
	{
		m_nEmotionExpressionID = nEmotionExpressionID;
	}

	public byte GetEventID()
	{
		return PhotonEvent.CS_RequestEmotionExpression;
	}
}

[Serializable]
public class CS_Ping : IPhotonEventMessage
{
    public int senderID { get; set; }
    public byte GetEventID()
	{
		return PhotonEvent.CS_Ping;
	}
}

[Serializable]
public class CS_FirstStatusSelection : IPhotonEventMessage
{
    public int senderID { get; set; }
    public int m_nEntityID;
	public FirstStatusElement m_FirstStatusElement;

	public CS_FirstStatusSelection(int nEntityID, FirstStatusElement firstStatusElement)
	{
		m_nEntityID = nEntityID;
		m_FirstStatusElement = firstStatusElement;
	}

	public byte GetEventID()
	{
		return PhotonEvent.CS_FirstStatusSelection;
	}
}

[Serializable]
public class CS_AbilitySelection : IPhotonEventMessage
{
    public int senderID { get; set; }
    public int m_nEntityID;
	public int m_nAbilityID;

	public CS_AbilitySelection(int nEntityID, int nAbilityID)
	{
		m_nEntityID = nEntityID;
		m_nAbilityID = nAbilityID;
	}

	public byte GetEventID()
	{
		return PhotonEvent.CS_AbilitySelection;
	}
}
#endregion
