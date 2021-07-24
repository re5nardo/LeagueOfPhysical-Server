using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using Entity;
using NetworkModel.Mirror;

public class PlayerInputController : MonoComponentBase
{
    private Queue<CS_NotifyMoveInputData> notifyMoveInputDatas = new Queue<CS_NotifyMoveInputData>();
    private Queue<CS_NotifyJumpInputData> notifyJumpInputDatas = new Queue<CS_NotifyJumpInputData>();

    public override void OnAttached(IEntity entity)
    {
        base.OnAttached(entity);

        TickPubSubService.AddSubscriber("EarlyTick", OnEarlyTick);
    }

    public override void OnDetached()
    {
        base.OnDetached();

        TickPubSubService.RemoveSubscriber("EarlyTick", OnEarlyTick);
    }

    public void AddMoveInputData(CS_NotifyMoveInputData notifyMoveInputData)
    {
        notifyMoveInputDatas.Enqueue(notifyMoveInputData);
    }

    public void AddJumpInputData(CS_NotifyJumpInputData notifyJumpInputData)
    {
        notifyJumpInputDatas.Enqueue(notifyJumpInputData);
    }

    private void OnEarlyTick(int tick)
    {
        if (notifyJumpInputDatas.Count > 0)
        {
            var notifyJumpInputData = notifyJumpInputDatas.Dequeue();

            if (CanJump())
            {
                var behaviorController = Entity.GetEntityComponent<BehaviorController>();
                behaviorController.Jump();
            }

            var processInputData = new SC_ProcessInputData();
            processInputData.tick = Game.Current.CurrentTick;
            processInputData.type = "jump";
            processInputData.sequence = notifyJumpInputData.jumpInputData.sequence;

            if (IDMap.TryGetConnectionIdByEntityId(notifyJumpInputData.jumpInputData.entityId, out var connectionId))
            {
                RoomNetwork.Instance.Send(processInputData, connectionId);
            }
        }

        if (notifyMoveInputDatas.Count > 0)
        {
            var notifyMoveInputData = notifyMoveInputDatas.Dequeue();

            if (notifyMoveInputData.playerMoveInput.inputType == PlayerMoveInput.InputType.Hold)
            {
                if (CanMove())
                {
                    var behaviorController = Entity.GetEntityComponent<BehaviorController>();
                    behaviorController.Move(Entity.Position + notifyMoveInputData.playerMoveInput.inputData.normalized * Game.Current.TickInterval * 5 * (Entity as Character).FactoredMovementSpeed);
                }
            }
            else if (notifyMoveInputData.playerMoveInput.inputType == PlayerMoveInput.InputType.Release)
            {
                var behaviorController = Entity.GetEntityComponent<BehaviorController>();
                behaviorController.StopBehavior(Define.MasterData.BehaviorID.MOVE);
            }

            SC_ProcessInputData processInputData = new SC_ProcessInputData();
            processInputData.tick = Game.Current.CurrentTick;
            processInputData.type = "move";
            processInputData.sequence = notifyMoveInputData.playerMoveInput.sequence;

            if (IDMap.TryGetConnectionIdByEntityId(notifyMoveInputData.playerMoveInput.entityId, out var connectionId))
            {
                RoomNetwork.Instance.Send(processInputData, connectionId);
            }
        }
    }

    private bool CanMove()
    {
        return true;
    }

    private bool CanJump()
    {
        //  IsGrounded는 클라의 그것을 믿는다. 서버에서 자체적으로 IsGrounded를 체크하면 클라와 미세한 차이에 의해 결과가 다를 수 있기에..
        //  그 대신 서버에서는 hold 여부와 같은 상태 체크를 한다. (To Do)
        return true;
    }
}
