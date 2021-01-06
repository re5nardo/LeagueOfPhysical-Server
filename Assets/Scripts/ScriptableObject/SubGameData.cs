using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SubGameData", menuName = "ScriptableObjects/SubGameData", order = 1)]
public class SubGameData : ScriptableObject
{
    public string id;
    public Object scene;
}
