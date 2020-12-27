using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubGameSelection : GameProcedureBase
{
    private List<string> sceneNames = new List<string>
    {
        "RememberGame",
    };

    public override IEnumerator Procedure()
    {
        var index = Random.Range(0, sceneNames.Count);
        
        GameProcedureBlackboard.keyValues["sceneName"] = sceneNames[index];

        yield break;
    }
}
