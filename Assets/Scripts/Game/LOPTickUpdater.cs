using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class LOPTickUpdater : TickUpdater
{
    protected override void OnInitElapsedTime()
    {
        ElapsedTime = (float)Mirror.NetworkTime.time;
    }

    protected override void OnUpdateElapsedTime()
    {
        ElapsedTime = (float)Mirror.NetworkTime.time;
    }
}