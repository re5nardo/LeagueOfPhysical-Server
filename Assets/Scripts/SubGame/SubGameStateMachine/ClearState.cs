using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;
using UnityEngine.SceneManagement;

namespace SubGameState
{
    public class ClearState : MonoStateBase
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
            if (!Enum.TryParse(input.ToString(), out SubGameStateInput subGameStateInput))
            {
                Debug.LogError($"Invalid input! input : {input}");
                return default;
            }

            switch (subGameStateInput)
            {
                case SubGameStateInput.StateDone:
                    return gameObject.GetOrAddComponent<SubGameState.EndState>();
            }

            throw new Exception($"Invalid transition: {GetType().Name} with {subGameStateInput}");
        }

        private IEnumerator Procedure()
        {
            yield return SceneManager.UnloadSceneAsync(LOP.Game.Current.MapData.sceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

            yield return SubGameBase.Current.Finalize();

            FSM.MoveNext(GameStateInput.StateDone);
        }
    }
}
