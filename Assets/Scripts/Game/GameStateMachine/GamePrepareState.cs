using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;
using System.Linq;
using NetworkModel.Mirror;

namespace GameState
{
    public class GamePrepareState : MonoStateBase
    {
        private Dictionary<string, float> playerPrepareStates = new Dictionary<string, float>();
        private bool resourceLoaded = false;

        public override void Enter()
        {
            base.Enter();

            SceneMessageBroker.AddSubscriber<CS_GamePreparation>(OnGamePreparation);

            //playerPrepareStates.Add(expectedUser, 0);

            StopCoroutine("Procedure");
            StartCoroutine("Procedure");
        }

        public override void Execute()
        {
            base.Execute();

            if (playerPrepareStates.All(x => x.Value > 1) && resourceLoaded)
            {
                FSM.MoveNext(GameStateInput.StateDone);
            }
        }

        public override void Exit()
        {
            base.Exit();

            SceneMessageBroker.RemoveSubscriber<CS_GamePreparation>(OnGamePreparation);

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
                    return gameObject.GetOrAddComponent<GameState.SubGameSelectionState>();
            }

            throw new Exception($"Invalid transition: {GetType().Name} with {gameStateInput}");
        }

        private IEnumerator Procedure()
        {
            //  Load SubGameSelection resource

            resourceLoaded = true;

            yield break;
        }

        private void OnGamePreparation(CS_GamePreparation gamePreparation)
        {
            if (!IsCurrent)
            {
                return;
            }

            var playerUserID = LOP.Game.Current.EntityIDPlayerUserID[gamePreparation.entityId];

            playerPrepareStates[playerUserID] = gamePreparation.preparation;
        }
    }

}
