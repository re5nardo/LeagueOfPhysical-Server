using UnityEngine;
using System;
using State;
using GameFramework;

public class StateFactory : MonoSingleton<StateFactory>
{
	public StateBase CreateState(GameObject entityGameObject, int masterDataId)
	{
		try
		{
			var masterData = MasterDataUtil.Get<StateMasterData>(masterDataId);

			return entityGameObject.AddComponent(Type.GetType(masterData.className)) as StateBase;
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message);
			return default;
		}
	}
}
