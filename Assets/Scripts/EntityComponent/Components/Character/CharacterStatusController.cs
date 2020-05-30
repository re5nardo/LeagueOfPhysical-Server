using GameFramework;
using System;
using System.Collections.Generic;
using Entity;

public class CharacterStatusController : MonoComponentBase, ISubscriber
{
	private Dictionary<Enum, Action<object[]>> m_dicMessageHandler = new Dictionary<Enum, Action<object[]>>();

	public override void OnAttached(IEntity entity)
	{
		base.OnAttached(entity);

        GamePubSubService.Instance.AddSubscriber(GameMessageKey.LevelUp, this);

		m_dicMessageHandler.Add(GameMessageKey.LevelUp, OnLevelUp);
	}

	public override void OnDetached()
	{
		base.OnDetached();

		if (GamePubSubService.IsInstantiated())
		{
            GamePubSubService.Instance.RemoveSubscriber(GameMessageKey.LevelUp, this);
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
            CharacterStatusData characterStatusData = Entity.GetComponent<CharacterStatusData>();
            characterStatusData.IncreaseSelectableFirstStatusCount();
		}
	}
	#endregion

	public void OnFirstStatusSelection(FirstStatusElement element)
	{
		Character character = Entity as Character;

		if (!character.IsSelectableFirstStatus)
			return;

        CharacterStatusData characterStatusData = Entity.GetComponent<CharacterStatusData>();
        characterStatusData.IncreaseFirstStatus(element);
        characterStatusData.DecreaseSelectableFirstStatusCount();
	}

	public void OnApplyFirstStatusAbility(FirstStatusElement element)
	{
        CharacterStatusData characterStatusData = Entity.GetComponent<CharacterStatusData>();
        characterStatusData.IncreaseFirstStatus(element);
	}
}
