using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BehaviorAttributeDispatcher
{
    private static Dictionary<Type, BehaviorAttributeHandler> handlerMap = new Dictionary<Type, BehaviorAttributeHandler>
    {
        {typeof(BehaviorAttribute), new BehaviorAttributeHandler()},
    };

    public static void Dispatch(Behavior.BehaviorBase behavior, BehaviorAttribute behaviorAttribute)
    {
        handlerMap[behaviorAttribute.GetType()].HandleAttribute(behavior, behaviorAttribute);
    }
}
