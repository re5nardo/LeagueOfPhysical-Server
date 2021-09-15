using UnityEngine;
using State;
using GameFramework;

public class StateController : LOPMonoEntityComponentBase
{
    public void StartState(int nStateMasterID, params object[] param)
    {
        StateBase state = StateFactory.Instance.CreateState(gameObject, nStateMasterID);
        Entity.AttachEntityComponent(state);
        state.SetData(nStateMasterID, param);
        state.onStateEnd += StateHelper.StateDestroyer;

        state.StartState();
    }
}
