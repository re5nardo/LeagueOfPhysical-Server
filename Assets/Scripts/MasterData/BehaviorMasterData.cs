using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(BehaviorMasterData), menuName = "ScriptableObjects/" + nameof(BehaviorMasterData))]
public class BehaviorMasterData : MasterDataBase
{
    public string className;
    public float lifespan = -1;
    public string[] classParams;
    public BehaviorMasterData[] compatibleBehaviors;
}
