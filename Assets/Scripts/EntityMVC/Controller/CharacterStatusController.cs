using GameFramework;
using System;
using System.Collections.Generic;
using Entity;

public class CharacterStatusController : MonoControllerComponentBase, ISubscriber
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

		if (Entity.EntityID == nEntityID)
		{
			CharacterStatusModel characterStatusModel = Entity.GetComponent<CharacterStatusModel>();
			characterStatusModel.IncreaseSelectableFirstStatusCount();
		}
	}
	#endregion

	public void OnFirstStatusSelection(FirstStatusElement element)
	{
		Character character = Entity as Character;

		if (!character.IsSelectableFirstStatus)
			return;

		CharacterStatusModel characterStatusModel = Entity.GetComponent<CharacterStatusModel>();
		characterStatusModel.IncreaseFirstStatus(element);
		characterStatusModel.DecreaseSelectableFirstStatusCount();
	}

	public void OnApplyFirstStatusAbility(FirstStatusElement element)
	{
		CharacterStatusModel characterStatusModel = Entity.GetComponent<CharacterStatusModel>();
		characterStatusModel.IncreaseFirstStatus(element);
	}
}
