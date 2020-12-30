using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameFramework.FSM;
using System;

public class SubGameClearState : MonoBehaviour, IState<GameStateInput>
{
    public IFiniteStateMachine<IState<GameStateInput>, GameStateInput> FSM => gameObject.GetOrAddComponent<GameStateMachine>();

    public void Enter()
    {
        StopCoroutine("Procedure");
        StartCoroutine("Procedure");
    }

    public void Execute()
    {
    }

    public void Exit()
    {
        StopCoroutine("Procedure");
    }

    public IState<GameStateInput> GetNext(GameStateInput input)
    {
        switch (input)
        {
            case GameStateInput.StateDone:
                return gameObject.GetOrAddComponent<MatchEndState>();
        }

        throw new Exception($"Invalid transition: {GetType().Name} with {input}");
    }

    private IEnumerator Procedure()
    {
        yield return SceneManager.UnloadSceneAsync(GameBlackboard.keyValues["sceneName"], UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

        GameBlackboard.keyValues.Remove("sceneName");

        FSM.MoveNext(GameStateInput.StateDone);
    }
}
