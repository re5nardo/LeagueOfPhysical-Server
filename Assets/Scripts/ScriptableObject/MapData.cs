using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObjects/MapData", order = 1)]
public class MapData : ScriptableObjectWrapper<MapData>
{
    public string mapId;
    public string mapName;
    public string description;
    public string sceneName;
    public int minPlayerCount = 2;
    public int maxPlayerCount = 8;
    public MapEnvironment mapEnvironment;
    public SpawnPoint[] spawnPoints = new SpawnPoint[8];
}

[Serializable]
public struct SpawnPoint
{
    public Vector3 position;
    public Vector3 rotation;
}
