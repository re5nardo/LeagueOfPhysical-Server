using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class CharacterStatusData : LOPMonoEntityComponentBase
{
    public FirstStatus firstStatus;
    public SecondStatus secondStatus;

    public int HP
    {
        get => secondStatus.HP;
        set => secondStatus.HP = Mathf.Min(value, secondStatus.MaximumHP);
    }

    public int MP
    {
        get => secondStatus.MP;
        set => secondStatus.MP = Mathf.Min(value, secondStatus.MaximumMP);
    }

    public int MaximumHP => secondStatus.MaximumHP;

    public float MovementSpeed => secondStatus.MovementSpeed;

    protected override void OnInitialize(EntityCreationData entityCreationData)
	{
		base.OnInitialize(entityCreationData);

        CharacterCreationData characterCreationData = entityCreationData as CharacterCreationData;

        firstStatus = characterCreationData.firstStatus;
        secondStatus = characterCreationData.secondStatus;
    }
}
