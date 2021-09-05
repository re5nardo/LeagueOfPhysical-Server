using GameFramework;
using System;
using System.Collections.Generic;
using Entity;

public class CharacterStatusController : MonoEntityComponentBase
{
	public override void OnAttached(IEntity entity)
	{
		base.OnAttached(entity);

        GamePubSubService.AddSubscriber(GameMessageKey.LevelUp, OnLevelUp);
	}

	public override void OnDetached()
	{
		base.OnDetached();

        GamePubSubService.RemoveSubscriber(GameMessageKey.LevelUp, OnLevelUp);
	}

	#region Message Handler
	private void OnLevelUp(object[] param)
	{
		int nEntityID = (int)param[0];

		if (Entity.EntityID == nEntityID)
		{
            CharacterStatusData characterStatusData = Entity.GetEntityComponent<CharacterStatusData>();
            characterStatusData.IncreaseSelectableFirstStatusCount();
		}
	}
	#endregion

	public void OnFirstStatusSelection(FirstStatusElement element)
	{
		Character character = Entity as Character;

		if (!character.IsSelectableFirstStatus)
			return;

        CharacterStatusData characterStatusData = Entity.GetEntityComponent<CharacterStatusData>();
        characterStatusData.IncreaseFirstStatus(element);
        characterStatusData.DecreaseSelectableFirstStatusCount();
	}

	public void OnApplyFirstStatusAbility(FirstStatusElement element)
	{
        CharacterStatusData characterStatusData = Entity.GetEntityComponent<CharacterStatusData>();
        characterStatusData.IncreaseFirstStatus(element);
	}
}
