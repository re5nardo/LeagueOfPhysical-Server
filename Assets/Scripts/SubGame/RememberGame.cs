using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RememberGame : SubGameBase
{
    [SerializeField] private string bgSceneName = "RiftOfSummoner";

    private const float TIME_LIMIT = 20;

    protected override IEnumerator OnInitialize()
    {
        yield break;
    }

    protected override IEnumerator OnFinalize()
    {
        yield break;
    }

    protected override void OnGameStart()
    {
    }

    protected override void OnGameEnd()
    {
    }

    protected override void OnTick(int tick)
    {
        if (ElapsedTime >= TIME_LIMIT)
        {
            EndGame();
        }
    }

    protected override void OnEarlyTickEnd(int tick)
    {
    }
}
