using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameFramework.FSM;
using System;

public class SubGamePrepareState : MonoBehaviour, IState<GameStateInput>
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
                return gameObject.GetOrAddComponent<SubGameProgressState>();
        }

        throw new Exception($"Invalid transition: {GetType().Name} with {input}");
    }

    private IEnumerator Procedure()
    {
        yield return SceneManager.LoadSceneAsync(GameBlackboard.keyValues["sceneName"], LoadSceneMode.Additive);

        yield return SubGameBase.Current.Initialize();

        FSM.MoveNext(GameStateInput.StateDone);
    }
}

//- ready(맵 로딩, 캐릭터 로딩 등등) 체크