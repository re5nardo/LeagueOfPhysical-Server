using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameProtocolDispatcher : MonoBehaviour
{
    private Dictionary<int, Action<IPhotonEventMessage>> protocolHandlers = new Dictionary<int, Action<IPhotonEventMessage>>();

    private void Awake()
    {
        protocolHandlers.Add(PhotonEvent.CS_NotifyPlayerLookAtPosition, CS_NotifyPlayerLookAtPositionHandler.Handle);
        protocolHandlers.Add(PhotonEvent.CS_NotifySkillInputData,       CS_NotifySkillInputDataHandler.Handle);
        protocolHandlers.Add(PhotonEvent.CS_RequestEmotionExpression,   CS_RequestEmotionExpressionHandler.Handle);
        protocolHandlers.Add(PhotonEvent.CS_FirstStatusSelection,       CS_FirstStatusSelectionHandler.Handle);
        protocolHandlers.Add(PhotonEvent.CS_AbilitySelection,           CS_AbilitySelectionHandler.Handle);
        protocolHandlers.Add(PhotonEvent.CS_NotifyMoveInputData,        CS_NotifyMoveInputDataHandler.Handle);
    }

    private void OnDestroy()
    {
        protocolHandlers.Clear();
    }

    public void DispatchProtocol(IPhotonEventMessage msg)
    {
        if (protocolHandlers.TryGetValue(msg.GetEventID(), out Action<IPhotonEventMessage> handler))
        {
            handler?.Invoke(msg);
        }
    }
}
