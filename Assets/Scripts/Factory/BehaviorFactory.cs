using UnityEngine;
using System;
using Behavior;
using GameFramework;

public class BehaviorFactory : MonoSingleton<BehaviorFactory>
{
    public BehaviorBase CreateBehavior(GameObject goTarget, int nBehaviorMasterID)
    {
        try
        {
            MasterData.Behavior masterData = MasterDataManager.instance.GetMasterData<MasterData.Behavior>(nBehaviorMasterID);

            switch (masterData.ClassName)
            {
                case "Move":
                    Move move = goTarget.AddComponent<Move>();
                    return move;

                case "Rotation":
                    Rotation rotation = goTarget.AddComponent<Rotation>();
                    return rotation;

                case "MeleeAttack":
                    MeleeAttack meleeAttack = goTarget.AddComponent<MeleeAttack>();
                    return meleeAttack;

                case "RangeAttack":
                    RangeAttack rangeAttack = goTarget.AddComponent<RangeAttack>();
                    return rangeAttack;
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
