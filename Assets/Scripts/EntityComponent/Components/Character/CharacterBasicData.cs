using UnityEngine;
using EntityCommand;

public class CharacterBasicData : EntityBasicData
{
    public string ModelId { get; private set; }
    private int m_nMasterDataID = -1;

	public override void Initialize(EntityCreationData entityCreationData)
	{
		base.Initialize(entityCreationData);

        CharacterCreationData characterCreationData = entityCreationData as CharacterCreationData;

        m_nMasterDataID = characterCreationData.masterDataId;

        ModelId = characterCreationData.modelId;

        Entity.SendCommandToViews(new ModelChanged(ModelId));
    }

	public int MasterDataID { get { return m_nMasterDataID; } }
}
