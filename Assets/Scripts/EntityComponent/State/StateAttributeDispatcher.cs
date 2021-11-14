using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateAttributeDispatcher
{
    private Dictionary<Type, StateAttributeHandler> handlerMap = new Dictionary<Type, StateAttributeHandler>
    {
        {typeof(StateAttribute), new StateAttributeHandler()},
    };

    public void Dispatch(State.StateBase state, StateAttribute stateAttribute)
    {
        handlerMap[stateAttribute.GetType()].HandleAttribute(state, stateAttribute);
    }
}
