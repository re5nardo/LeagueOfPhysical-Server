using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameFramework.FSM;
using System;

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
                return gameObject.GetOrAddComponent<SubGameProgressState>();
        }

        throw new Exception($"Invalid transition: {GetType().Name} with {gameStateInput}");
    }

    private IEnumerator Procedure()
    {
        yield return SceneManager.LoadSceneAsync(LOP.Game.Current.GameManager.SubGameData.sceneName, LoadSceneMode.Additive);

        yield return SubGameBase.Current.Initialize();

        FSM.MoveNext(GameStateInput.StateDone);
    }
}

//- ready(맵 로딩, 캐릭터 로딩 등등) 체크