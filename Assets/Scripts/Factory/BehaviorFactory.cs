using UnityEngine;
using System;
using Behavior;
using GameFramework;
using System.Collections.Generic;

public class BehaviorFactory : MonoSingleton<BehaviorFactory>
{
    private List<Type> source = new List<Type>
    {
        typeof(Move),
        typeof(Rotation),
        typeof(MeleeAttack),
        typeof(RangeAttack),
    };

    public BehaviorBase CreateBehavior(GameObject goTarget, int nBehaviorMasterID)
    {
        try
        {
            MasterData.Behavior masterData = MasterDataManager.instance.GetMasterData<MasterData.Behavior>(nBehaviorMasterID);

            Type target = source.Find(type => type.Name == masterData.ClassName);
            if (target != null)
            {
                return goTarget.AddComponent(target) as BehaviorBase;
            }

            Debug.LogError(string.Format("There is no matched ClassName! masterData.ClassName : {0}", masterData.ClassName));
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            return null;
        }
    }
}
