using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;
using System.Linq;
using GameFramework;

public class GamePrepareState : GameStateBase
{
    private Dictionary<string, float> playerPrepareStates = new Dictionary<string, float>();
    private bool resourceLoaded = false;

    protected override void OnEnter()
    {
        roomProtocolDispatcher[typeof(CS_GamePreparation)] = OnGamePreparation;

        if (PhotonNetwork.room.ExpectedUsers != null)
        {
            foreach (var expectedUser in PhotonNetwork.room.ExpectedUsers)
            {
                playerPrepareStates.Add(expectedUser, 0);
            }
        }

        StopCoroutine("Procedure");
        StartCoroutine("Procedure");
    }

    protected override void OnExecute()
    {
        if (playerPrepareStates.All(x => x.Value > 1) && resourceLoaded)
        {
            FSM.MoveNext(GameStateInput.StateDone);
        }
    }

    protected override void OnExit()
    {
        StopCoroutine("Procedure");
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

    private IEnumerator Procedure()
    {
        //  Load SubGameSelection resource

        resourceLoaded = true;

        yield break;
    }

    private void OnGamePreparation(IMessage msg)
    {
        CS_GamePreparation gamePreparation = msg as CS_GamePreparation;

        var playerUserID = LOP.Game.Current.EntityIDPlayerUserID[gamePreparation.entityID];

        playerPrepareStates[playerUserID] = gamePreparation.preparation;
    }
}
