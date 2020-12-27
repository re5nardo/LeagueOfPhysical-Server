using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubGameClear : GameProcedureBase
{
    public override IEnumerator Procedure()
    {
        yield return SceneManager.UnloadSceneAsync(GameProcedureBlackboard.keyValues["sceneName"], UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

        GameProcedureBlackboard.keyValues.Remove("sceneName");
    }
}
