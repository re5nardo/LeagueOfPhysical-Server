using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;
using UnityEngine.SceneManagement;
using NetworkModel.Mirror;
using GameFramework;
using System.Linq;

namespace GameState
{
    public class SubGamePrepareState : MonoStateBase
    {
        private const int WAIT_TIMEOUT = 3;

        private Dictionary<string, float> playerPrepares = new Dictionary<string, float>();

        public override void OnEnter()
        {
            SceneMessageBroker.AddSubscriber<CS_SubGamePreparation>(OnSubGamePreparation);

            LOP.Room.Instance.ExpectedPlayerList?.ForEach(expectedPlayer =>
            {
                playerPrepares[expectedPlayer] = 0;
            });
        }

        public override IEnumerator OnExecute()
        {
            var subGameLoader = SceneManager.LoadSceneAsync(LOP.Game.Current.SubGameData.sceneName, LoadSceneMode.Additive);
            var mapLoader = SceneManager.LoadSceneAsync(LOP.Game.Current.MapData.sceneName, LoadSceneMode.Additive);

            yield return new WaitUntil(() => subGameLoader.isDone && mapLoader.isDone);

            yield return SubGameBase.Current.Initialize();

            yield return new WaitForSeconds(1);

            yield return new WaitForDone(() => playerPrepares.All(x => x.Value >= 1), WAIT_TIMEOUT);

            using var disposer = PoolObjectDisposer<SC_SubGameReadyNotice>.Get();
            var subGameReadyNotice = disposer.PoolObject;

            subGameReadyNotice.timeBeforeStart = 3;
            RoomNetwork.Instance.SendToAll(subGameReadyNotice);

            yield return new WaitForSeconds(1);

            subGameReadyNotice.timeBeforeStart = 2;
            RoomNetwork.Instance.SendToAll(subGameReadyNotice);

            yield return new WaitForSeconds(1);

            subGameReadyNotice.timeBeforeStart = 1;
            RoomNetwork.Instance.SendToAll(subGameReadyNotice);

            yield return new WaitForSeconds(1);

            FSM.MoveNext(GameStateInput.StateDone);
        }

        public override void OnExit()
        {
            SceneMessageBroker.RemoveSubscriber<CS_SubGamePreparation>(OnSubGamePreparation);
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
                case GameStateInput.StateDone: return gameObject.GetOrAddComponent<GameState.SubGameProgressState>();
            }

            throw new Exception($"Invalid transition: {GetType().Name} with {gameStateInput}");
        }

        private void OnSubGamePreparation(CS_SubGamePreparation subGamePreparation)
        {
            if (!IsCurrent)
            {
                return;
            }

            if (LOP.Game.Current.EntityIDPlayerUserID.TryGetValue(subGamePreparation.entityId, out var playerUserID))
            {
                playerPrepares[playerUserID] = subGamePreparation.preparation;
            }
        }
    }
}
