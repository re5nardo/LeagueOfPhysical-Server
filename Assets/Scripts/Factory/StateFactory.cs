using UnityEngine;
using System;
using State;
using GameFramework;

public class StateFactory : MonoSingleton<StateFactory>
{
	public StateBase CreateState(GameObject goTarget, int nStateMasterID)
	{
		try
		{
			MasterData.State masterData = MasterDataManager.instance.GetMasterData<MasterData.State>(nStateMasterID);

			switch (masterData.ClassName)
			{
				case "BasicState":
                    BasicState basicState = goTarget.AddComponent<BasicState>();
					return basicState;

                case "EntitySelfDestroy":
                    EntitySelfDestroy entitySelfDestroy = goTarget.AddComponent<EntitySelfDestroy>();
                    return entitySelfDestroy;
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
