using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubGamePrepare : GameProcedureBase
{
    public override IEnumerator Procedure()
    {
        yield return SceneManager.LoadSceneAsync(GameProcedureBlackboard.keyValues["sceneName"], LoadSceneMode.Additive);

        yield return SubGameBase.Current.Initialize();
    }
}

//- ready(맵 로딩, 캐릭터 로딩 등등) 체크