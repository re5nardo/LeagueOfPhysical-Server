using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using Entity;

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

            SC_ProcessInputData processInputData = new SC_ProcessInputData();
            processInputData.tick = Game.Current.CurrentTick;
            processInputData.type = "jump";
            processInputData.sequence = notifyJumpInputData.jumpInputData.sequence;

            RoomNetwork.Instance.Send(processInputData, notifyJumpInputData.senderID);
        }

        if (notifyMoveInputDatas.Count > 0)
        {
            var notifyMoveInputData = notifyMoveInputDatas.Dequeue();

            if (notifyMoveInputData.m_PlayerMoveInput.inputType == PlayerMoveInput.InputType.Hold)
            {
                if (CanMove())
                {
                    var behaviorController = Entity.GetEntityComponent<BehaviorController>();
                    behaviorController.Move(Entity.Position + notifyMoveInputData.m_PlayerMoveInput.inputData.ToVector3().normalized * Game.Current.TickInterval * 5 * (Entity as Character).MovementSpeed);
                }
            }
            else if (notifyMoveInputData.m_PlayerMoveInput.inputType == PlayerMoveInput.InputType.Release)
            {
                var behaviorController = Entity.GetEntityComponent<BehaviorController>();
                behaviorController.StopBehavior(Define.MasterData.BehaviorID.MOVE);
            }

            SC_ProcessInputData processInputData = new SC_ProcessInputData();
            processInputData.tick = Game.Current.CurrentTick;
            processInputData.type = "move";
            processInputData.sequence = notifyMoveInputData.m_PlayerMoveInput.sequence;

            RoomNetwork.Instance.Send(processInputData, notifyMoveInputData.senderID);
        }
    }

    private bool CanMove()
    {
        return true;
    }

    private bool CanJump()  //  BehaviorController::Move() 에서 체크해야 하나...? 
    {
        return true;
    }
}
