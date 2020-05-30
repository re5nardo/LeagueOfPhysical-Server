using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class CharacterAbilityData : MonoComponentBase
{
	private int m_nSelectableAbilityCount;
	private List<int> m_SelectableAbilityIDs = new List<int>();

	public int SelectableAbilityCount { get { return m_nSelectableAbilityCount; } }

	public void IncreaseSelectableAbilityCount()
	{
		m_nSelectableAbilityCount++;

		m_SelectableAbilityIDs = AbilityManager.Instance.GenerateAbilityIDs(Entity.EntityID);
		
		RoomNetwork.Instance.Send(new SC_SelectableAbilityInfo(m_SelectableAbilityIDs), PhotonHelper.GetActorID(Entity.EntityID));
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
