using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;

public class WaitForPlayersState : MonoBehaviour, IState<GameStateInput>
{
    public IFiniteStateMachine<IState<GameStateInput>, GameStateInput> FSM => gameObject.GetOrAddComponent<GameStateMachine>();

    public void Enter()
    {
    }

    public void Execute()
    {
        if (PhotonNetwork.room.ExpectedUsers == null || PhotonNetwork.room.ExpectedUsers.Length == LOP.Game.Current.PlayerUserIDPhotonPlayer.Count)
        {
            FSM.MoveNext(GameStateInput.StateDone);
        }
    }

    public void Exit()
    {
    }

    public IState<GameStateInput> GetNext(GameStateInput input)
    {
        switch (input)
        {
            case GameStateInput.StateDone:
                return gameObject.GetOrAddComponent<SubGameSelectionState>();
        }

        throw new Exception($"Invalid transition: {GetType().Name} with {input}");
    }
}
