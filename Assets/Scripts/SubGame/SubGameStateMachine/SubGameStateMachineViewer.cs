using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SubGameStateMachine))]
public class SubGameStateMachineViewer : MonoBehaviour
{
    public SubGameStateMachine SubGameStateMachine { get; private set; }

    private void Awake()
    {
        SubGameStateMachine = GetComponent<SubGameStateMachine>();
    }
}
