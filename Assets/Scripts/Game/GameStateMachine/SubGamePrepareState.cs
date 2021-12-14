using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;
using UnityEngine.SceneManagement;

namespace GameState
{
    public class SubGamePrepareState : MonoStateBase
    {
        public override void Enter()
        {
            base.Enter();

            StopCoroutine("Procedure");
            StartCoroutine("Procedure");
        }

        public override void Exit()
        {
            base.Exit();

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
                    return gameObject.GetOrAddComponent<GameState.SubGameProgressState>();
            }

            throw new Exception($"Invalid transition: {GetType().Name} with {gameStateInput}");
        }

        private IEnumerator Procedure()
        {
            yield return SceneManager.LoadSceneAsync(LOP.Game.Current.SubGameData.sceneName, LoadSceneMode.Additive);

            FSM.MoveNext(GameStateInput.StateDone);
        }
    }
}
