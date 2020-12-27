using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubGameProgress : GameProcedureBase
{
    public override IEnumerator Procedure()
    {
        SubGameBase.Current.StartGame();

        yield return new WaitUntil(() => SubGameBase.Current.IsGameEnd);
    }
}
