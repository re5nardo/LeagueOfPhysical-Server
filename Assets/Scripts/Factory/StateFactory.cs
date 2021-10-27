using UnityEngine;
using System;
using State;
using GameFramework;

public class StateFactory : MonoSingleton<StateFactory>
{
	public StateBase CreateState(GameObject entityGameObject, int stateMasterId)
	{
		try
		{
			var masterData = ScriptableObjects.Get<StateMasterData>(x => x.id == stateMasterId);

			return entityGameObject.AddComponent(Type.GetType(masterData.className)) as StateBase;
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message);
			return default;
		}
	}
}
