using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FlapWang : SubGameBase
{
    protected override IEnumerator OnInitialize()
    {
        Physics.gravity *= LOP.Game.Current.GameManager.MapData.mapEnvironment.GravityFactor;

        yield break;
    }

    protected override IEnumerator OnFinalize()
    {
        Physics.gravity /= LOP.Game.Current.GameManager.MapData.mapEnvironment.GravityFactor;

        yield break;
    }

    protected override void OnGameStart()
    {
        phase = Phase.ready;
    }

    protected override void OnGameEnd()
    {
    }

    protected override void OnTick(int tick)
    {
        if (ElapsedTime >= TIME_WAIT_PLAYERS)
        {

        }

        if (ElapsedTime >= TIME_LIMIT)
        {
            EndGame();
        }
    }

    protected override void OnEarlyTickEnd(int tick)
    {
    }
}
