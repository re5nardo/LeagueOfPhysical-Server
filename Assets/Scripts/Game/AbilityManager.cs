using System.Collections.Generic;
using UnityEngine;
using Entity;
using GameFramework;

public class AbilityManager : MonoSingleton<AbilityManager>
{
	public List<int> GenerateAbilityIDs(int nEntityID)
	{
		List<int> abilityIDs = new List<int>();

		abilityIDs.Add(Random.Range(0, 7));
		abilityIDs.Add(Random.Range(0, 7));
		abilityIDs.Add(Random.Range(0, 7));
		abilityIDs.Add(Random.Range(0, 7));
		abilityIDs.Add(Random.Range(0, 7));

		return abilityIDs;
	}

	public void ApplyAbility(int nEntityID, int nAbilityID)
	{
		IEntity entity = Entities.Get(nEntityID);
		CharacterStatusController characterStatusController = entity.GetComponent<CharacterStatusController>();

		MasterData.Ability master = MasterDataManager.Instance.GetMasterData<MasterData.Ability>(nAbilityID);

		//	Dummy...
		switch(master.Name)
		{
			case "AllStatus":
				characterStatusController.OnApplyFirstStatusAbility(FirstStatusElement.STR);
				characterStatusController.OnApplyFirstStatusAbility(FirstStatusElement.DEX);
				characterStatusController.OnApplyFirstStatusAbility(FirstStatusElement.CON);
				characterStatusController.OnApplyFirstStatusAbility(FirstStatusElement.INT);
				characterStatusController.OnApplyFirstStatusAbility(FirstStatusElement.WIS);
				characterStatusController.OnApplyFirstStatusAbility(FirstStatusElement.CHA);
				return;
			case "STRStatus": characterStatusController.OnApplyFirstStatusAbility(FirstStatusElement.STR); return;
			case "DEXStatus": characterStatusController.OnApplyFirstStatusAbility(FirstStatusElement.DEX); return;
			case "CONStatus": characterStatusController.OnApplyFirstStatusAbility(FirstStatusElement.CON); return;
			case "INTStatus": characterStatusController.OnApplyFirstStatusAbility(FirstStatusElement.INT); return;
			case "WISStatus": characterStatusController.OnApplyFirstStatusAbility(FirstStatusElement.WIS); return;
			case "CHAStatus": characterStatusController.OnApplyFirstStatusAbility(FirstStatusElement.CHA); return;
		}
	}
}
