using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOP
{
    public class GameManager : MonoBehaviour
    {
        private GameStateMachine gameStateMachine = null;

        public string subGameId;
        public string mapName;
        public SubGameData SubGameData => SubGameData.Get(subGameId);

        public bool IsGameEnd => gameStateMachine.CurrentState is GameEndState;

        private void Awake()
        {
            gameStateMachine = new GameObject("GameStateMachine").AddComponent<GameStateMachine>();
        }

        private void OnDestroy()
        {
            if (gameStateMachine != null)
            {
                Destroy(gameStateMachine.gameObject);
            }
        }

        public void StartGame()
        {
            gameStateMachine.StartStateMachine();
        }
    }
}
