using System.Collections.Generic;
using System;

public class RoomPubSubService
{
    private static SimplePubSubService<Enum, object> simplePubSubService = new SimplePubSubService<Enum, object>();

    public static void Publish(Enum key, object value)
    {
        simplePubSubService.Publish(key, value);
    }

    public static void AddSubscriber(Enum key, Action<object> subscriber)
    {
        simplePubSubService.AddSubscriber(key, subscriber);
    }

    public static void RemoveSubscriber(Enum key, Action<object> subscriber)
    {
        simplePubSubService.RemoveSubscriber(key, subscriber);
    }
}
