using UnityEngine;
using Entity;

public class CharacterBuilder : EntityBuilder<CharacterBuilder, Character, CharacterCreationData>
{
    protected override CharacterCreationData entityCreationData { get; set; } = new CharacterCreationData();

    public CharacterBuilder SetMasterDataId(int masterDataId)
    {
        entityCreationData.masterDataId = masterDataId;
        return this;
    }

	public CharacterBuilder SetModelId(string modelId)
    {
        entityCreationData.modelId = modelId;
        return this;
    }

    public CharacterBuilder SetFirstStatus(FirstStatus firstStatus)
    {
        entityCreationData.firstStatus = firstStatus;
        return this;
    }

	public CharacterBuilder SetSecondStatus(SecondStatus secondStatus)
	{
        entityCreationData.secondStatus = secondStatus;
        return this;
	}

    public override Character Build()
    {
        GameObject goCharacter = new GameObject(string.Format("Entity_{0}", entityCreationData.entityId));
        Character character = goCharacter.AddComponent<Character>();

        character.Initialize(entityCreationData);

        EntityManager.Instance.RegisterEntity(character);

		return character;
    }
}
