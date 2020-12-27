using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RememberGame : SubGameBase
{
    [SerializeField] private Object sceneToLoad = null;
    [SerializeField] private SpawnManager spawnManager = null;

    protected override IEnumerator OnInitialize()
    {
        yield return SceneManager.LoadSceneAsync(sceneToLoad.name, LoadSceneMode.Additive);
    }

    protected override void OnGameStart()
    {
        spawnManager.StartSpawn();
    }

    protected override void OnGameEnd()
    {
    }

    protected override void OnTick(int tick)
    {
    }

    protected override void OnEarlyTickEnd(int tick)
    {
    }
}
