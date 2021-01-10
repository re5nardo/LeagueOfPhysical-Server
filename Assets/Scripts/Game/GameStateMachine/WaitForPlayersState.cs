using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;

public class WaitForPlayersState : GameStateBase
{
    public override void Execute()
    {
        if (PhotonNetwork.room.ExpectedUsers == null || PhotonNetwork.room.ExpectedUsers.Length == LOP.Game.Current.PlayerUserIDPhotonPlayer.Count)
        {
            FSM.MoveNext(GameStateInput.StateDone);
        }
    }

    public override IState GetNext<I>(I input)
    {
        if (!Enum.TryParse(input.ToString(), out GameStateInput gameStateInput))
        {
            Debug.LogError($"Invalid input! input : {input}");
            return default;
        }

        switch (gameStateInput)
        {
            case GameStateInput.StateDone:
                return gameObject.GetOrAddComponent<SubGameSelectionState>();
        }

        throw new Exception($"Invalid transition: {GetType().Name} with {gameStateInput}");
    }
}
