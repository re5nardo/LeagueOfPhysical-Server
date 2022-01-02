using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameFramework.FSM;
using System;

namespace GameState
{
    public class SubGameClearState : MonoStateBase
    {
        public override IEnumerator OnExecute()
        {
            yield return SubGameBase.Current.Finalize();

            var subGameUnloader = SceneManager.UnloadSceneAsync(LOP.Game.Current.SubGameData.sceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            var mapUnloader = SceneManager.UnloadSceneAsync(LOP.Game.Current.MapData.sceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

            yield return new WaitUntil(() => subGameUnloader.isDone && mapUnloader.isDone);

            FSM.MoveNext(GameStateInput.StateDone);
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
                case GameStateInput.StateDone: return gameObject.GetOrAddComponent<GameState.SubGameEndState>();
            }

            throw new Exception($"Invalid transition: {GetType().Name} with {gameStateInput}");
        }
    }
}
