using UnityEngine;
using System;
using Skill;
using GameFramework;
using System.Collections.Generic;

public class SkillFactory : MonoSingleton<SkillFactory>
{
    private List<Type> source = new List<Type>
    {
        typeof(FireBehavior),
        typeof(PlasmaFission),
    };

    public SkillBase CreateSkill(GameObject goTarget, int nSkillMasterID)
    {
        try
        {
            MasterData.Skill masterData = MasterDataManager.instance.GetMasterData<MasterData.Skill>(nSkillMasterID);

            Type target = source.Find(type => type.Name == masterData.ClassName);
            if (target != null)
            {
                return goTarget.AddComponent(target) as SkillBase;
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
