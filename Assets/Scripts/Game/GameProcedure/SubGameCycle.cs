using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubGameCycle : GameProcedureBase
{
    private List<GameProcedureBase> gameProcedures = new List<GameProcedureBase>();

    private void Awake()
    {
        gameProcedures.Add(gameObject.AddComponent<SubGameSelection>());
        gameProcedures.Add(gameObject.AddComponent<SubGamePrepare>());
        gameProcedures.Add(gameObject.AddComponent<SubGameProgress>());
        gameProcedures.Add(gameObject.AddComponent<SubGameClear>());
    }

    public override IEnumerator Procedure()
    {
        foreach (var gameProcedure in gameProcedures)
        {
            yield return gameProcedure.Procedure();
        }
    }
}
