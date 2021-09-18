using GameFramework;
using System;
using System.Collections.Generic;

public class CharacterAbilityController : LOPMonoEntityComponentBase
{
	#region Message Handler
	private void OnLevelUp(object[] param)
	{
		int nEntityID = (int)param[0];
		int nLevel = (int)param[1];

		if (Entity.EntityID == nEntityID && (nLevel % 3) == 0)
		{
            CharacterAbilityData characterAbilityData = Entity.GetEntityComponent<CharacterAbilityData>();
            characterAbilityData.IncreaseSelectableAbilityCount();
		}
	}
	#endregion

	public void OnAbilitySelection(int nAbilityID)
	{
        CharacterAbilityData characterAbilityData = Entity.GetEntityComponent<CharacterAbilityData>();
        characterAbilityData.SelectAbility(nAbilityID);
        characterAbilityData.DecreaseSelectableAbilityCount();
	}
}
