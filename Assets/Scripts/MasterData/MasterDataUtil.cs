using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class MasterDataUtil
{
    public static T Get<T>(int id) where T : MasterDataBase
    {
        return ScriptableObjectUtil.Get<T>(x => x.id == id);
    }
}
