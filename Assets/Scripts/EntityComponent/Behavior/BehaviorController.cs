using UnityEngine;
using Behavior;
using State;
using GameFramework;

public class BehaviorController : LOPMonoEntityComponentBase
{
    public void Move(Vector3 vec3Destination)
    {
        Vector3 vec3Direction = vec3Destination - Entity.Position;

        Move oldMove = Entity.GetEntityComponent<Move>();
        if (oldMove != null)
        {
            oldMove.SetDestination(vec3Destination);
        }
        else
        {
            Move move = BehaviorFactory.Instance.CreateBehavior(gameObject, Define.MasterData.BehaviorID.MOVE) as Move;
            Entity.AttachEntityComponent(move);
            move.Initialize(new MoveBehaviorParam(Define.MasterData.BehaviorID.MOVE, vec3Destination));
            move.onBehaviorEnd += BehaviorHelper.BehaviorDestroyer;

            move.StartBehavior();
        }

        Rotation oldRotation = Entity.GetEntityComponent<Rotation>();
        if (oldRotation != null)
        {
            oldRotation.Direction = vec3Direction;
        }
        else
        {
            Rotation rotation = BehaviorFactory.Instance.CreateBehavior(gameObject, Define.MasterData.BehaviorID.ROTATION) as Rotation;
            Entity.AttachEntityComponent(rotation);
            rotation.Initialize(new RotationBehaviorParam(Define.MasterData.BehaviorID.ROTATION, vec3Direction));
            rotation.onBehaviorEnd += BehaviorHelper.BehaviorDestroyer;

            rotation.StartBehavior();
        }
    }

    public void Jump()
    {
        Jump jump = BehaviorFactory.Instance.CreateBehavior(gameObject, Define.MasterData.BehaviorID.JUMP) as Jump;
        Entity.AttachEntityComponent(jump);
        jump.Initialize(new BehaviorParam(Define.MasterData.BehaviorID.JUMP));
        jump.onBehaviorEnd += BehaviorHelper.BehaviorDestroyer;

        jump.StartBehavior();
    }

    public void Die()
    {
        var behaviors = Entity.GetEntityComponents<BehaviorBase>();
        foreach (var behavior in behaviors)
        {
            if (behavior.IsPlaying)
                behavior.StopBehavior();
        }

        var states = Entity.GetEntityComponents<StateBase>();
        foreach (var state in states)
        {
            if (state.IsPlaying)
                state.StopState();
        }
    }

    public void StartBehavior(BehaviorParam behaviorParam)
    {
        BehaviorBase behavior = BehaviorFactory.Instance.CreateBehavior(gameObject, behaviorParam.masterDataId);
        Entity.AttachEntityComponent(behavior);
        behavior.Initialize(behaviorParam);
        behavior.onBehaviorEnd += BehaviorHelper.BehaviorDestroyer;

        behavior.StartBehavior();
    }

    public void StopBehavior(int nBehaviorMasterID)
    {
        var behaviors = Entity.GetEntityComponents<BehaviorBase>();

        behaviors?.ForEach(behavior =>
        {
            if (behavior.MasterDataId == nBehaviorMasterID)
            {
                behavior.StopBehavior();
            }
        });
    }
}
