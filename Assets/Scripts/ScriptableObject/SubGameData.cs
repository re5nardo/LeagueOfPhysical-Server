using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SubGameData", menuName = "ScriptableObjects/SubGameData", order = 1)]
public class SubGameData : ScriptableObjectWrapper<SubGameData>
{
    public string title;
    public string description;
    public string sceneName;
    public int minPlayers = 2;
    public MatchType[] availableMatchType;
}
