﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOP
{
    public class GameManager : MonoBehaviour
    {
        private GameStateMachine gameStateMachine = null;

        public SubGameData currentSubGame = null;

        public bool IsMatchEnd => gameStateMachine.CurrentState is MatchEndState;

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

        public void StartGameManager()
        {
            gameStateMachine.StartStateMachine();
        }
    }
}