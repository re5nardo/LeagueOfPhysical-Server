using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using Entity;

public class PlayerMoveInputController : MonoComponentBase
{
    private Queue<CS_NotifyMoveInputData> notifyMoveInputDatas = new Queue<CS_NotifyMoveInputData>();

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

    public void AddPlayerMoveInputController(CS_NotifyMoveInputData notifyMoveInputData)
    {
        notifyMoveInputDatas.Enqueue(notifyMoveInputData);
    }

    private void OnEarlyTick(int tick)
    {
        if (notifyMoveInputDatas.Count == 0)
            return;

        var notifyMoveInputData = notifyMoveInputDatas.Dequeue();

        if (notifyMoveInputData.m_PlayerMoveInput.inputType == PlayerMoveInput.InputType.Hold)
        {
            if (CanMove())
            {
                var behaviorController = Entity.GetComponent<BehaviorController>();
                behaviorController.Move(Entity.Position + notifyMoveInputData.m_PlayerMoveInput.inputData.ToVector3().normalized * Game.Current.TickInterval * 3 * (Entity as Character).MovementSpeed, notifyMoveInputData.m_PlayerMoveInput.sequence);
            }
        }
        else if (notifyMoveInputData.m_PlayerMoveInput.inputType == PlayerMoveInput.InputType.Release)
        {
            var behaviorController = Entity.GetComponent<BehaviorController>();
            behaviorController.StopBehavior(Define.MasterData.BehaviorID.MOVE);
        }
    }

    private bool CanMove()  //  BehaviorController::Move() 에서 체크해야 하나...? 
    {
        return true;
    }
}
