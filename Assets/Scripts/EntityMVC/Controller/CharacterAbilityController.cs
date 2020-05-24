using GameFramework;
using System;
using System.Collections.Generic;

public class CharacterAbilityController : MonoControllerComponentBase, ISubscriber
{
	private Dictionary<Enum, Action<object[]>> m_dicMessageHandler = new Dictionary<Enum, Action<object[]>>();

	public override void OnAttached(IEntity entity)
	{
		base.OnAttached(entity);

        RoomPubSubService.Instance.AddSubscriber(MessageKey.LevelUp, this);

		m_dicMessageHandler.Add(MessageKey.LevelUp, OnLevelUp);
	}

	public override void OnDetached()
	{
		base.OnDetached();

		if (RoomPubSubService.IsInstantiated())
		{
            RoomPubSubService.Instance.RemoveSubscriber(MessageKey.LevelUp, this);
		}

		m_dicMessageHandler.Clear();
	}

	#region ISubscriber
	public void OnMessage(Enum key, params object[] param)
	{
		m_dicMessageHandler[key](param);
	}
	#endregion

	#region Message Handler
	private void OnLevelUp(params object[] param)
	{
		int nEntityID = (int)param[0];
		int nLevel = (int)param[1];

		if (Entity.EntityID == nEntityID && (nLevel % 3) == 0)
		{
			CharacterAbilityModel characterAbilityModel = Entity.GetComponent<CharacterAbilityModel>();
			characterAbilityModel.IncreaseSelectableAbilityCount();
		}
	}
	#endregion

	public void OnAbilitySelection(int nAbilityID)
	{
		CharacterAbilityModel characterAbilityModel = Entity.GetComponent<CharacterAbilityModel>();
		characterAbilityModel.SelectAbility(nAbilityID);
		characterAbilityModel.DecreaseSelectableAbilityCount();
	}
}
