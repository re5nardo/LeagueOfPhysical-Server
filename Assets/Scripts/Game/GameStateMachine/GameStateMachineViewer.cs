using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameStateMachine))]
public class GameStateMachineViewer : MonoBehaviour
{
    public GameStateMachine GameStateMachine { get; private set; }

    private void Awake()
    {
        GameStateMachine = GetComponent<GameStateMachine>();
    }
}
