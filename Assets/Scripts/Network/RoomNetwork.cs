using UnityEngine;
using System;
using GameFramework;

public class RoomNetwork : MonoSingleton<RoomNetwork>, INetwork
{
	public INetworkImpl NetworkImpl { get; private set; }

	public Action<IMessage> OnMessage
	{
		get => NetworkImpl.OnMessage;
        set => NetworkImpl.OnMessage = value;
    }

	protected override void Awake()
	{
		base.Awake();

        NetworkImpl = gameObject.AddComponent<RoomNetworkImpl_PUN>();
	}

	public void Send(IMessage msg, int targetId, bool reliable = true, bool instant = false)
	{
        NetworkImpl.Send(msg, targetId, reliable, instant);
	}

    public void Send(IMessage msg, ulong targetId, bool reliable = true, bool instant = false)
    {
        NetworkImpl.Send(msg, targetId, reliable, instant);
    }

    public void SendToAll(IMessage msg, bool reliable = true, bool instant = false)
	{
        NetworkImpl.SendToAll(msg, reliable, instant);
	}

	public void SendToNear(IMessage msg, Vector3 center, float radius, bool reliable = true, bool instant = false)
	{
        NetworkImpl.SendToNear(msg, center, radius, reliable, instant);
	}
}
