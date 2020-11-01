using UnityEngine;
using System;
using State;
using GameFramework;
using System.Collections.Generic;

public class StateFactory : MonoSingleton<StateFactory>
{
    private List<Type> source = new List<Type>
    {
        typeof(BasicState),
        typeof(EntitySelfDestroy),
    };

	public StateBase CreateState(GameObject goTarget, int nStateMasterID)
	{
		try
		{
			MasterData.State masterData = MasterDataManager.instance.GetMasterData<MasterData.State>(nStateMasterID);

            Type target = source.Find(type => type.Name == masterData.ClassName);
            if (target != null)
            {
                return goTarget.AddComponent(target) as StateBase;
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
