using UnityEngine;
using State;

public class StateController : LOPMonoEntityComponentBase
{
    public void StartState(StateParam stateParam)
    {
        var oldState = Entity.GetState(stateParam.masterDataId);
        if (oldState == null)
        {
            StateBase state = StateFactory.Instance.CreateState(gameObject, stateParam.masterDataId);
            Entity.AttachEntityComponent(state);
            state.onStateEnd += StateHelper.StateDestroyer;
            state.Initialize(stateParam);
            state.StartState();
            return;
        }

        var masterData = MasterDataUtil.Get<StateMasterData>(stateParam.masterDataId);
        switch (masterData.overlapResolveType)
        {
            case OverlapResolveType.AllowMultiple:
                {
                    StateBase state = StateFactory.Instance.CreateState(gameObject, stateParam.masterDataId);
                    Entity.AttachEntityComponent(state);
                    state.onStateEnd += StateHelper.StateDestroyer;
                    state.Initialize(stateParam);
                    state.StartState();
                    break;
                }

            case OverlapResolveType.UseOld:
                break;

            case OverlapResolveType.UseNew:
                {
                    oldState.StopState();

                    StateBase state = StateFactory.Instance.CreateState(gameObject, stateParam.masterDataId);
                    Entity.AttachEntityComponent(state);
                    state.onStateEnd += StateHelper.StateDestroyer;
                    state.Initialize(stateParam);
                    state.StartState();
                    break;
                }

            case OverlapResolveType.Accumulate:
                oldState.OnAccumulate(stateParam);
                break;
        }
    }
}
