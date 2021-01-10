using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameFramework.FSM;
using System;

public class SubGameClearState : GameStateBase
{
    public override void Enter()
    {
        StopCoroutine("Procedure");
        StartCoroutine("Procedure");
    }

    public override void Exit()
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
                return gameObject.GetOrAddComponent<MatchEndState>();
        }

        throw new Exception($"Invalid transition: {GetType().Name} with {gameStateInput}");
    }

    private IEnumerator Procedure()
    {
        yield return SceneManager.UnloadSceneAsync(LOP.Game.Current.GameManager.currentSubGame.scene.name, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

        LOP.Game.Current.GameManager.currentSubGame = null;

        FSM.MoveNext(GameStateInput.StateDone);
    }
}
