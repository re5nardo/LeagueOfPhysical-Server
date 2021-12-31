using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

[Serializable]
public class SubGameStateSyncData : ISyncData
{
    public string state;

    public SubGameStateSyncData() { }

    public SubGameStateSyncData(string state)
    {
        this.state = state;
    }
}
