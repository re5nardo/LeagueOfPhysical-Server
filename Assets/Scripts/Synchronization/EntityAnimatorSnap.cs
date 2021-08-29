using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

[Serializable]
public class EntityAnimatorSnap : ISnap
{
    public int Tick { get; set; }
    public int entityId;
    public float animatorSpeed;
    public AnimationParametersData animationParametersData = new AnimationParametersData();
    public List<AnimStateData> animStateDataList = new List<AnimStateData>();

    public EntityAnimatorSnap() { }

    public bool EqualsCore(ISnap snap)
    {
        return false;
    }

    public bool EqualsValue(ISnap snap)
    {
        return false;
    }

    public ISnap Clone()
    {
        EntityAnimatorSnap clone = new EntityAnimatorSnap();

        clone.Tick = Tick;
        clone.entityId = entityId;
        clone.animatorSpeed = animatorSpeed;
        clone.animationParametersData = animationParametersData.Clone();
        animStateDataList?.ForEach(animStateData =>
        {
            clone.animStateDataList.Add(animStateData.Clone());
        });

        return clone;
    }
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
