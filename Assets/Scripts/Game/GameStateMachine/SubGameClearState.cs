using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameFramework.FSM;
using System;

public class SubGameClearState : MonoStateBase
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
                return gameObject.GetOrAddComponent<GameEndState>();
        }

        throw new Exception($"Invalid transition: {GetType().Name} with {gameStateInput}");
    }

    private IEnumerator Procedure()
    {
        yield return SceneManager.UnloadSceneAsync(LOP.Game.Current.GameManager.SubGameData.sceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

        LOP.Game.Current.GameManager.subGameId = null;
        LOP.Game.Current.GameManager.mapId = null;

        FSM.MoveNext(GameStateInput.StateDone);
    }
}
