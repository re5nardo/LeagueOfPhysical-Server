using System.Collections.Generic;
using System;
using GameFramework;

public class RoomPubSubService : Singleton<RoomPubSubService>, IPubSubService
{
    private Dictionary<Enum, HashSet<ISubscriber>> m_dicSubscribers = new Dictionary<Enum, HashSet<ISubscriber>>();

    public void Publish(Enum key, params object[] param)
    {
		HashSet<ISubscriber> hashSubscriber = null;
		if (m_dicSubscribers.TryGetValue(key, out hashSubscriber))
		{
			foreach (ISubscriber subscriber in hashSubscriber)
			{
				subscriber.OnMessage(key, param);
			}
		}
	}

    public void AddSubscriber(Enum key, ISubscriber subscriber)
    {
        if (!m_dicSubscribers.ContainsKey(key))
        {
			m_dicSubscribers.Add(key, new HashSet<ISubscriber>());
        }

		m_dicSubscribers[key].Add(subscriber);
    }

    public void RemoveSubscriber(Enum key, ISubscriber subscriber)
    {
		m_dicSubscribers[key].Remove(subscriber);
    }
}
