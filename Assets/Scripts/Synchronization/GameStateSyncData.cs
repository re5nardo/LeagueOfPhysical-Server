using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

[Serializable]
public class GameStateSyncData : ISyncData
{
    public string state;

    public GameStateSyncData() { }

    public GameStateSyncData(string state)
    {
        this.state = state;
    }
}
