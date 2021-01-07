﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;
using GameFramework;

public class SubGameSelectionState : MonoBehaviour, IState<GameStateInput>
{
    public IFiniteStateMachine<IState<GameStateInput>, GameStateInput> FSM => gameObject.GetOrAddComponent<GameStateMachine>();

    private SubGameData[] subGameDatas = null;

    public void Enter()
    {
        subGameDatas = Resources.LoadAll<SubGameData>("ScriptableObject/SubGameData");

        var index = UnityEngine.Random.Range(0, subGameDatas.Length);

        LOP.Game.Current.GameManager.currentSubGame = subGameDatas[index];

        FSM.MoveNext(GameStateInput.StateDone);
    }

    public void Execute()
    {
    }

    public void Exit()
    {
    }

    public IState<GameStateInput> GetNext(GameStateInput input)
    {
        switch (input)
        {
            case GameStateInput.StateDone:
                return gameObject.GetOrAddComponent<SubGamePrepareState>();
        }

        throw new Exception($"Invalid transition: {GetType().Name} with {input}");
    }
}
