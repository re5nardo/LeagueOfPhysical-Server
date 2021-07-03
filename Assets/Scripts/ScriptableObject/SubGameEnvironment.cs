using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SubGameEnvironment", menuName = "ScriptableObjects/SubGameEnvironment", order = 1)]
public class SubGameEnvironment : ScriptableObjectWrapper<SubGameEnvironment>
{
    public float MoveSpeedFactor = 1;
    public float JumpPowerFactor = 1;
    public float GravityFactor = 1;
}
