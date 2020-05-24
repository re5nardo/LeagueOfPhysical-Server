using UnityEngine;
using System;
using GameFramework;

public class RoomNetwork : MonoSingleton<RoomNetwork>, INetwork
{
	private INetworkImpl m_INetworkImpl__ = null;
	public INetworkImpl m_NetworkImpl
	{
		get { return m_INetworkImpl__; }
		set { m_INetworkImpl__ = value; }
	}

	public Action<IMessage, object[]> onMessage
	{
		get { return m_NetworkImpl.onMessage; }
		set { m_NetworkImpl.onMessage = value; }
	}

	protected override void Awake()
	{
		base.Awake();

		m_INetworkImpl__ = gameObject.AddComponent<RoomNetworkImpl>();
	}

	public void Send(IMessage msg, int nTargetID, bool bReliable = true, bool bInstant = false)
	{
		m_NetworkImpl.Send(msg, nTargetID, bReliable, bInstant);
	}

	public void SendToAll(IMessage msg, bool bReliable = true, bool bInstant = false)
	{
		m_NetworkImpl.SendToAll(msg, bReliable, bInstant);
	}

	public void SendToNear(IMessage msg, Vector3 vec3Center, float fRadius, bool bReliable = true, bool bInstant = false)
	{
		m_NetworkImpl.SendToNear(msg, vec3Center, fRadius, bReliable, bInstant);
	}
}
