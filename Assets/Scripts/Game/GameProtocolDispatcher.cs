using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class GameProtocolDispatcher : MonoBehaviour
{
    private Dictionary<int, IHandler<IPhotonEventMessage>> m_dicProtocolHandler = new Dictionary<int, IHandler<IPhotonEventMessage>>();

    private void Awake()
    {
        m_dicProtocolHandler.Add(PhotonEvent.CS_NotifyPlayerLookAtPosition, new CS_NotifyPlayerLookAtPositionHandler());
        m_dicProtocolHandler.Add(PhotonEvent.CS_NotifySkillInputData, new CS_NotifySkillInputDataHandler());
        m_dicProtocolHandler.Add(PhotonEvent.CS_RequestEmotionExpression, new CS_RequestEmotionExpressionHandler());
        m_dicProtocolHandler.Add(PhotonEvent.CS_FirstStatusSelection, new CS_FirstStatusSelectionHandler());
        m_dicProtocolHandler.Add(PhotonEvent.CS_AbilitySelection, new CS_AbilitySelectionHandler());
        m_dicProtocolHandler.Add(PhotonEvent.CS_NotifyMoveInputData, new CS_NotifyMoveInputDataHandler());
    }

    private void OnDestroy()
    {
        m_dicProtocolHandler.Clear();
    }

    public void DispatchProtocol(IPhotonEventMessage msg)
    {
        if (m_dicProtocolHandler.ContainsKey(msg.GetEventID()))
        {
            m_dicProtocolHandler[msg.GetEventID()].Handle(msg);
        }
    }
}
