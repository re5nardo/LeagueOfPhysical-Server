using System.Collections.Generic;
using UnityEngine;
using System;
using Entity;
using GameFramework;

public class PlayerMoveInputManager : MonoBehaviour, ISubscriber
{
	[SerializeField] private float correctValue = 5f;

	private Dictionary<int, Action<IMessage, int>> dicNetworkMessageHandler = new Dictionary<int, Action<IMessage, int>>();
	private Dictionary<Enum, Action<object[]>> dicPubSubMessageHandler = new Dictionary<Enum, Action<object[]>>();

	private Dictionary<int, List<CS_NotifyMoveInputData>> dicPlayerMoveInputData = new Dictionary<int, List<CS_NotifyMoveInputData>>();

	#region MonoBehaviour
	private void Awake()
	{
		dicNetworkMessageHandler.Add(PhotonEvent.CS_NotifyMoveInputData, OnCS_NotifyMoveInputData);

		RoomNetwork.Instance.onMessage += OnNetworkMessage;

		dicPubSubMessageHandler.Add(RoomMessageKey.PlayerEnter, OnPlayerEnter);
		dicPubSubMessageHandler.Add(RoomMessageKey.PlayerLeave, OnPlayerLeave);

        RoomPubSubService.Instance.AddSubscriber(RoomMessageKey.PlayerEnter, this);
        RoomPubSubService.Instance.AddSubscriber(RoomMessageKey.PlayerLeave, this);
	}

	private void OnDestroy()
	{
		if (RoomNetwork.IsInstantiated())
		{
			RoomNetwork.Instance.onMessage -= OnNetworkMessage;
		}

		dicNetworkMessageHandler.Clear();

		if (RoomPubSubService.IsInstantiated())
		{
            RoomPubSubService.Instance.RemoveSubscriber(RoomMessageKey.PlayerEnter, this);
            RoomPubSubService.Instance.RemoveSubscriber(RoomMessageKey.PlayerLeave, this);
		}

		dicPubSubMessageHandler.Clear();
	}

	private void Update()
	{
		foreach (var moveInputData in dicPlayerMoveInputData)
		{
			ProcessMoveInputData(moveInputData.Key, moveInputData.Value);

			moveInputData.Value.Clear();
		}
	}
	#endregion

	//	조금 더 고도화 해야하려나..?
	private void ProcessMoveInputData(int entityID, List<CS_NotifyMoveInputData> notifyMoveInputDatas)
	{
		Vector3 inputSum = Vector3.zero;
		foreach (var notifyMoveInputData in notifyMoveInputDatas)
		{
			if (notifyMoveInputData.m_PlayerMoveInput.m_InputType == PlayerMoveInput.InputType.Release)
			{
				Character character = EntityManager.Instance.GetEntity(notifyMoveInputData.m_PlayerMoveInput.m_nEntityID) as Character;
				Behavior.Move oldMove = character.GetComponent<Behavior.Move>();
				oldMove?.StopBehavior();
			}
			else if (notifyMoveInputData.m_PlayerMoveInput.m_InputType == PlayerMoveInput.InputType.Hold)
			{
				inputSum += notifyMoveInputData.m_PlayerMoveInput.m_InputData;
			}
		}

		if (inputSum.sqrMagnitude > 0)
		{
			Character character = EntityManager.Instance.GetEntity(entityID) as Character;
			character.Move(character.Position + inputSum * correctValue * character.MovementSpeed);
		}
	}

	#region Network Message Handler
	private void OnNetworkMessage(IMessage msg, object[] objects)
	{
		int nEventID = (msg as IPhotonEventMessage).GetEventID();
		int nSenderID = (int)objects[0];

		if (dicNetworkMessageHandler.ContainsKey(nEventID))
		{
			dicNetworkMessageHandler[nEventID](msg, nSenderID);
		}
	}

	private void OnCS_NotifyMoveInputData(IMessage msg, int nSenderID)
	{
		CS_NotifyMoveInputData notifyMoveInputData = msg as CS_NotifyMoveInputData;

		if (!dicPlayerMoveInputData.ContainsKey(notifyMoveInputData.m_PlayerMoveInput.m_nEntityID))
		{
			dicPlayerMoveInputData.Add(notifyMoveInputData.m_PlayerMoveInput.m_nEntityID, new List<CS_NotifyMoveInputData>());
		}

		dicPlayerMoveInputData[notifyMoveInputData.m_PlayerMoveInput.m_nEntityID].Add(notifyMoveInputData);
	}
	#endregion

	#region PubSub Message Handler
	public void OnMessage(Enum key, params object[] param)
	{
		dicPubSubMessageHandler[key](param);
	}

	public void OnPlayerEnter(params object[] param)
	{
		int entityID = (int)param[0];

		//dicPlayerMoveInputData.Add(entityID, null);
	}

	public void OnPlayerLeave(params object[] param)
	{
		int entityID = (int)param[0];

		//dicPlayerMoveInputData.Remove(entityID);
	}
	#endregion
}
