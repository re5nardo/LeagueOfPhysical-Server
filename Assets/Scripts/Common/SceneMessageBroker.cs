using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

public class SceneMessageBroker : MonoSingleton<SceneMessageBroker>
{
    public SimplePubSubService DefaultMessageBroker { get; } = new SimplePubSubService();

    protected override void OnDestroy()
    {
        DefaultMessageBroker.Clear();
    }

    public static void Publish<T>(T message)
    {
        Instance.DefaultMessageBroker.Publish(message);
    }

    public static void Publish(Type type, object message)
    {
        Instance.DefaultMessageBroker.Publish(type, message);
    }

    public static void AddSubscriber<T>(Action<T> subscriber)
    {
        Instance.DefaultMessageBroker.AddSubscriber(subscriber);
    }

    public static void RemoveSubscriber<T>(Action<T> subscriber)
    {
        Instance.DefaultMessageBroker.RemoveSubscriber(subscriber);
    }
}
