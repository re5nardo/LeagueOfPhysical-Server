using UnityEngine;
using State;
using GameFramework;

public class StateController : LOPMonoEntityComponentBase
{
    public void StartState(StateParam stateParam)
    {
        StateBase state = StateFactory.Instance.CreateState(gameObject, stateParam.masterDataId);
        Entity.AttachEntityComponent(state);
        state.Initialize(stateParam);
        state.onStateEnd += StateHelper.StateDestroyer;

        state.StartState();
    }
}
