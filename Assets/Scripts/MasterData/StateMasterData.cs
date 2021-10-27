using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(StateMasterData), menuName = "ScriptableObjects/" + nameof(StateMasterData))]
public class StateMasterData : MasterDataBase
{
    public string className;
    public float lifespan = -1;
    public string[] classParams;
}
