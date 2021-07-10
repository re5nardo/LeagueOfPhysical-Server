using UnityEngine;
using System;
using GameFramework;

public class RoomNetworkImpl : MonoBehaviour, INetworkImpl
{
    public Action<IMessage> OnMessage { get; set; }

	private void Awake()
	{
		PhotonNetwork.OnEventCall += OnEvent;
	}

	private void OnDestroy()
	{
        OnMessage = null;

		PhotonNetwork.OnEventCall -= OnEvent;
	}

	public void Send(IMessage msg, int targetId, bool reliable = true, bool instant = false)
	{
		IPhotonEventMessage eventMsg = msg as IPhotonEventMessage;
        eventMsg.senderID = PhotonNetwork.player.ID;

		PhotonNetwork.RaiseEvent(eventMsg.GetEventID(), msg, reliable, new RaiseEventOptions { TargetActors = new int[] { targetId } });

		if (instant)
		{
			PhotonNetwork.SendOutgoingCommands();
		}
	}

	public void SendToAll(IMessage msg, bool reliable = true, bool instant = false)
	{
		foreach (var player in PhotonNetwork.playerList)
		{
			Send(msg, player.ID, reliable, instant);
		}
	}

	public void SendToNear(IMessage msg, Vector3 center, float radius, bool reliable = true, bool instant = false)
	{
		foreach (IEntity entity in Entities.Get(center, radius, EntityRole.Player))
		{
			if (LOP.Game.Current.EntityIDPlayerUserID.TryGetValue(entity.EntityID, out string strPlayerUserID))
			{
				if (LOP.Game.Current.PlayerUserIDPhotonPlayer.TryGetValue(strPlayerUserID, out WeakReference target))
				{
					if (!target.IsAlive)
					{
						continue;
					}

					PhotonPlayer photonPlayer = target.Target as PhotonPlayer;

					Send(msg, photonPlayer.ID, reliable, instant);
				}
			}
		}
	}

	#region PhotonEvent
	private void OnEvent(byte eventcode, object content, int senderId)
    {
        IPhotonEventMessage photonEventMessage = content as IPhotonEventMessage;
        photonEventMessage.senderID = senderId;

        OnMessage?.Invoke(photonEventMessage);
    }
    #endregion
}
