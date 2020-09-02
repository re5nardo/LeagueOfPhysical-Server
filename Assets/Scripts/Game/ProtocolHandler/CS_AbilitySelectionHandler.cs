using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class CS_AbilitySelectionHandler
{
    public static void Handle(IPhotonEventMessage msg)
    {
        CS_AbilitySelection abilitySelection = msg as CS_AbilitySelection;

        CharacterAbilityController abilityController = Entities.Get(abilitySelection.m_nEntityID).GetComponent<CharacterAbilityController>();
        abilityController.OnAbilitySelection(abilitySelection.m_nAbilityID);
    }
}
