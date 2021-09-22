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
        throw new NotImplementedException();
	}
}
