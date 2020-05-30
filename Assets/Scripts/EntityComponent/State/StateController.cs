using UnityEngine;
using State;
using GameFramework;

public class StateController : MonoComponentBase
{
    public void StartState(int nStateMasterID, params object[] param)
    {
        StateBase state = StateFactory.Instance.CreateState(gameObject, nStateMasterID);
        Entity.AttachComponent(state);
        state.SetData(nStateMasterID, param);
        state.onStateEnd += StateHelper.StateDestroyer;

        state.StartState();
    }
}
