using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class StateAttributeHandler
{
    public virtual void HandleAttribute(State.StateBase state, StateAttribute stateAttribute)
    {
    }
}

public class SubClassStateAttributeHandler : StateAttributeHandler
{
    public override void HandleAttribute(State.StateBase state, StateAttribute stateAttribute)
    {
        var subClassStateAttribute = stateAttribute as SubClassStateAttribute;

        subClassStateAttribute.classNames?.ForEach(className =>
        {
            var subComponent = state.Entity.AttachEntityComponent(className);

            state.onStateEnd += state =>
            {
                state.Entity.DetachEntityComponent(subComponent);

                if (subComponent is Component component)
                {
                    Object.Destroy(component);
                }
            };
        });
    }
}
