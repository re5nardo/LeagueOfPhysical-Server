using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

[Serializable]
public class AnimatorSyncData : ISyncData
{
    public float animatorSpeed;
    public AnimationParametersData animationParametersData = new AnimationParametersData();
    public List<AnimStateData> animStateDataList = new List<AnimStateData>();
}

[Serializable]
public class AnimStateData
{
    public int stateHash;
    public float normalizedTime;
    public int layerId;
    public float weight;

    public AnimStateData Clone()
    {
        AnimStateData clone = new AnimStateData();

        clone.stateHash = stateHash;
        clone.normalizedTime = normalizedTime;
        clone.layerId = layerId;
        clone.weight = weight;

        return clone;
    }
}

[Serializable]
public class AnimationParametersData
{
    public List<object> values = new List<object>();

    public AnimationParametersData Clone()
    {
        AnimationParametersData clone = new AnimationParametersData();

        clone.values = new List<object>(values);

        return clone;
    }
}
