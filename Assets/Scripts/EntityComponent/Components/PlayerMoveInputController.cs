using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using Entity;

public class PlayerMoveInputController : MonoComponentBase, ITickable
{
    private Queue<CS_NotifyMoveInputData> notifyMoveInputDatas = new Queue<CS_NotifyMoveInputData>();

    public void AddPlayerMoveInputController(CS_NotifyMoveInputData notifyMoveInputData)
    {
        notifyMoveInputDatas.Enqueue(notifyMoveInputData);
    }

    public void Tick(int tick)
    {
        if (notifyMoveInputDatas.Count == 0)
            return;

        var notifyMoveInputData = notifyMoveInputDatas.Dequeue();

        //  Send to client
        SC_PlayerMoveInputResponse playerMoveInputResponse = new SC_PlayerMoveInputResponse();
        playerMoveInputResponse.m_nTick = Game.Current.CurrentTick;
        playerMoveInputResponse.m_nEntityID = Entity.EntityID;
        playerMoveInputResponse.m_Position = Entity.Position;
        playerMoveInputResponse.m_Rotation = Entity.Rotation;
        playerMoveInputResponse.m_lLastProcessedSequence = notifyMoveInputData.m_PlayerMoveInput.sequence;

        RoomNetwork.Instance.Send(playerMoveInputResponse, PhotonHelper.GetActorID(Entity.EntityID), true, true);

        if (CanMove())
        {
            //  Move process
            var behaviorController = Entity.GetComponent<BehaviorController>();
            behaviorController.Move(Entity.Position + notifyMoveInputData.m_PlayerMoveInput.inputData.ToVector3().normalized * Game.Current.TickInterval * 3 * (Entity as Character).MovementSpeed);
        }
    }

    private bool CanMove()  //  BehaviorController::Move() 에서 체크해야 하나...? 
    {
        return true;
    }
}
