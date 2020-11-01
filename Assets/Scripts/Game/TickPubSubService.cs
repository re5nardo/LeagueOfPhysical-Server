using System.Collections.Generic;
using System;
using GameFramework;

public class TickPubSubService
{
    private static SimplePubSubService<string, int> simplePubSubService = new SimplePubSubService<string, int>();

    public static void Publish(string key, int value)
    {
        simplePubSubService.Publish(key, value);
    }

    public static void AddSubscriber(string key, Action<int> subscriber)
    {
        simplePubSubService.AddSubscriber(key, subscriber);
    }

    public static void RemoveSubscriber(string key, Action<int> subscriber)
    {
        simplePubSubService.RemoveSubscriber(key, subscriber);
    }
}
