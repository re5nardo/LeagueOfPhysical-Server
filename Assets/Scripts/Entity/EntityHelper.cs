using UnityEngine;
using GameFramework;
using Entity;

public class EntityHelper
{
    public static EntitySnapInfo GetEntitySnapInfo(IEntity entity)
	{
		if(entity is MonoEntityBase)
		{
			return (entity as MonoEntityBase).GetEntitySnapInfo();
		}

		Debug.LogWarning("entity type is not vaild! type : " + entity.GetType());
		return null;
	}

    public static Character CreatePlayerCharacter(int nCharacterID)
    {
        MasterData.Character characterMasterData = MasterDataManager.Instance.GetMasterData<MasterData.Character>(nCharacterID);
        MasterData.FirstStatus firstStatusMasterData = MasterDataManager.Instance.GetMasterData<MasterData.FirstStatus>(characterMasterData.FirstStatusID);
        MasterData.SecondStatus secondStatusMasterData = MasterDataManager.Instance.GetMasterData<MasterData.SecondStatus>(characterMasterData.SecondStatusID);

        Rect mapRect = LOP.Game.Current.GetMapRect();
        Vector3 vec3StartPosition = new Vector3(UnityEngine.Random.Range(mapRect.xMin + 5, mapRect.xMax - 5), 0, UnityEngine.Random.Range(mapRect.yMin + 5, mapRect.yMax - 5));
        Vector3 vec3StartRotation = new Vector3(0, UnityEngine.Random.Range(0, 360), 0);

        FirstStatus firstStatus = new FirstStatus(firstStatusMasterData);

        return Character.Builder()
            .SetMasterDataID(nCharacterID)
            .SetPosition(vec3StartPosition)
            .SetRotation(vec3StartRotation)
            .SetVelocity(Vector3.zero)
            .SetAngularVelocity(Vector3.zero)
            .SetModelPath(characterMasterData.ModelResID)
            .SetFirstStatus(firstStatus)
            .SetSecondStatus(new SecondStatus(firstStatus, secondStatusMasterData))
            .SetSelectableFirstStatusCount(8)
            .SetEntityRole(EntityRole.Player)
            .Build();
    }
}
