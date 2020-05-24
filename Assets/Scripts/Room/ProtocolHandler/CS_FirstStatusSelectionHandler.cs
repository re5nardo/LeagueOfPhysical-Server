using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class CS_FirstStatusSelectionHandler : IHandler<IPhotonEventMessage>
{
    public void Handle(IPhotonEventMessage msg)
    {
        CS_FirstStatusSelection firstStatusSelection = msg as CS_FirstStatusSelection;

        CharacterStatusController statusController = EntityManager.Instance.GetEntity(firstStatusSelection.m_nEntityID).GetComponent<CharacterStatusController>();
        statusController.OnFirstStatusSelection(firstStatusSelection.m_FirstStatusElement);
    }
}
