using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = nameof(StateMasterData), menuName = "ScriptableObjects/" + nameof(StateMasterData))]
public class StateMasterData : MasterDataBase
{
    public string className;
    public float lifespan = -1;
    public string[] classParams;
    public OverlapResolveType overlapResolveType;
    public CrowdControl[] crowdControls;
}

[Serializable]
public enum CrowdControl
{
    //  Movement modifiers
    Stun = 0,

    //  Action/ability rate modifiers

    //  Forced action modifiers
}

[Serializable]
public enum OverlapResolveType
{
    UseNew = 0,
    UseOld = 1,
    AllowMultiple = 2,
    Accumulate = 3,
}
