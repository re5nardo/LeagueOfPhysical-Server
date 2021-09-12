using UnityEngine;
using EntityCommand;

public class CharacterBasicData : EntityBasicData
{
    public int MasterDataId { get; private set; } = -1;

    private string modelId;
    public string ModelId
    {
        get => modelId;
        private set
        {
            modelId = value;
            Entity.SendCommandToViews(new ModelChanged(value));
        }
    }

	public override void Initialize(EntityCreationData entityCreationData)
	{
		base.Initialize(entityCreationData);

        CharacterCreationData characterCreationData = entityCreationData as CharacterCreationData;

        MasterDataId = characterCreationData.masterDataId;
        ModelId = characterCreationData.modelId;
    }
}
