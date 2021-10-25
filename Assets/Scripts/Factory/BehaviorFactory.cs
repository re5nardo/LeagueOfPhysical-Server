using UnityEngine;
using System;
using Behavior;
using GameFramework;

public class BehaviorFactory : MonoSingleton<BehaviorFactory>
{
    public BehaviorBase CreateBehavior(GameObject entityGameObject, int behaviorMasterId)
    {
        try
        {
            var masterData = ScriptableObjects.Get<BehaviorMasterData>(x => x.id == behaviorMasterId);

            return entityGameObject.AddComponent(Type.GetType(masterData.className)) as BehaviorBase;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return default;
        }
    }
}
