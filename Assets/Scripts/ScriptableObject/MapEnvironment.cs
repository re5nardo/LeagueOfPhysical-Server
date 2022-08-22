using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

[CreateAssetMenu(fileName = "MapEnvironment", menuName = "ScriptableObjects/MapEnvironment", order = 1)]
public class MapEnvironment : ScriptableObjectWrapper<MapEnvironment>
{
    public float MoveSpeedFactor = 1;
    public float JumpPowerFactor = 1;
    public float GravityFactor = 1;
}
