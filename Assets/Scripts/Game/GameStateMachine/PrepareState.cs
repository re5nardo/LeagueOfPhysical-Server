using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;
using System.Linq;
using NetworkModel.Mirror;
using GameFramework;

namespace GameState
{
    public class PrepareState : MonoStateBase
    {
        private const int WAIT_TIMEOUT = 3;

        private Dictionary<string, float> playerPrepares = new Dictionary<string, float>();

        public override void OnEnter()
        {
            SceneMessageBroker.AddSubscriber<CS_GamePreparation>(OnGamePreparation);

            LOP.Room.Instance.ExpectedPlayerList?.ForEach(expectedPlayer =>
            {
                playerPrepares[expectedPlayer] = 0;
            });
        }

        public override IEnumerator OnExecute()
        {
            //  Load game resource
            //  ...

            yield return new WaitForDone(() => playerPrepares.All(x => x.Value >= 1), WAIT_TIMEOUT);

            FSM.MoveNext(GameStateInput.StateDone);
        }

        public override void OnExit()
        {
            SceneMessageBroker.RemoveSubscriber<CS_GamePreparation>(OnGamePreparation);
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
                case GameStateInput.StateDone: return gameObject.GetOrAddComponent<GameState.SubGameSelectionState>();
            }

            throw new Exception($"Invalid transition: {GetType().Name} with {gameStateInput}");
        }

        private void OnGamePreparation(CS_GamePreparation gamePreparation)
        {
            if (!IsCurrent)
            {
                return;
            }

            if (LOP.Game.Current.EntityIDPlayerUserID.TryGetValue(gamePreparation.entityId, out var playerUserID))
            {
                playerPrepares[playerUserID] = gamePreparation.preparation;
            }
        }
    }
}
