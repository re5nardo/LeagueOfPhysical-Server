using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FSM;
using System;

public class SubGameSelectionState : MonoBehaviour, IState<GameStateInput>
{
    public IFiniteStateMachine<IState<GameStateInput>, GameStateInput> FSM => gameObject.GetOrAddComponent<GameStateMachine>();

    public void Enter()
    {
        List<string> sceneNames = new List<string>
        {
            "RememberGame",
        };

        var index = UnityEngine.Random.Range(0, sceneNames.Count);

        GameBlackboard.keyValues["sceneName"] = sceneNames[index];

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
