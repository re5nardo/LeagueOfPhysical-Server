using UnityEngine;
using System;
using Skill;
using GameFramework;

public class SkillFactory : MonoSingleton<SkillFactory>
{
    public SkillBase CreateSkill(GameObject goTarget, int nSkillMasterID)
    {
        try
        {
            MasterData.Skill masterData = MasterDataManager.instance.GetMasterData<MasterData.Skill>(nSkillMasterID);

            switch (masterData.ClassName)
            {
                case "FireBehavior":
                    FireBehavior fireBehavior = goTarget.AddComponent<FireBehavior>();
                    return fireBehavior;

                case "PlasmaFission":
                    PlasmaFission plasmaFission = goTarget.AddComponent<PlasmaFission>();
                    return plasmaFission;
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
