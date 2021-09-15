using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

public class CharacterAbilityData : LOPMonoEntityComponentBase
{
	private int m_nSelectableAbilityCount;
	private List<int> m_SelectableAbilityIDs = new List<int>();

	public int SelectableAbilityCount { get { return m_nSelectableAbilityCount; } }

	public void IncreaseSelectableAbilityCount()
	{
        throw new NotImplementedException();
	}

	public void DecreaseSelectableAbilityCount()
	{
		m_nSelectableAbilityCount--;
	}

	public void SelectAbility(int nAbilityID)
	{
		if (m_nSelectableAbilityCount == 0)
		{
			Debug.LogWarning("Can't select Ability. m_nSelectableAbilityCount is 0!");
			return;
		}

		if(!m_SelectableAbilityIDs.Contains(nAbilityID))
		{
			Debug.LogWarning("nAbilityID is invalid! nAbilityID : " + nAbilityID);
			return;
		}

		AbilityManager.Instance.ApplyAbility(Entity.EntityID, nAbilityID);
	}
}
