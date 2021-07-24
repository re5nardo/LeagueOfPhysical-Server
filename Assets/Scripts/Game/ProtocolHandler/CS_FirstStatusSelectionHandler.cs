﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using NetworkModel.PUN;

public class CS_FirstStatusSelectionHandler
{
    public static void Handle(IMessage msg)
    {
        CS_FirstStatusSelection firstStatusSelection = msg as CS_FirstStatusSelection;

        CharacterStatusController statusController = Entities.Get(firstStatusSelection.m_nEntityID).GetEntityComponent<CharacterStatusController>();
        statusController.OnFirstStatusSelection(firstStatusSelection.m_FirstStatusElement);
    }
}
