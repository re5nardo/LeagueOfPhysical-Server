using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

[CreateAssetMenu(fileName = "LOPSettings", menuName = "ScriptableObjects/LOPSettings", order = 1)]
public class LOPSettings : ScriptableObjectWrapper<LOPSettings>
{
    public bool connectLocalServer;
}
