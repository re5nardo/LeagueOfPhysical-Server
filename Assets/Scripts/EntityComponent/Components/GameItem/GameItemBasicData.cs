using UnityEngine;
using Entity;
using EntityCommand;
using GameFramework;

public class GameItemBasicData : EntityBasicData
{
    public string ModelId { get; private set; }
    private int m_nMasterDataID = -1;
	private int m_nHP = 0;
	private int m_nMaximumHP = 0;

	public override void Initialize(EntityCreationData entityCreationData)
	{
		base.Initialize(entityCreationData);

        GameItemCreationData gameItemCreationData = entityCreationData as GameItemCreationData;

        m_nMasterDataID = gameItemCreationData.masterDataId;

        var masterData = MasterDataManager.Instance.GetMasterData<MasterData.GameItem>(m_nMasterDataID);
		m_nHP = m_nMaximumHP = masterData.HP;

        ModelId = gameItemCreationData.modelId;

        Entity.SendCommandToViews(new ModelChanged(ModelId));
    }

	public int MasterDataID { get { return m_nMasterDataID; } }

	public int CurrentHP
	{
		get { return m_nHP; }
		set { m_nHP = Mathf.Min(value, m_nMaximumHP); }
	}

	public int MaximumHP { get { return m_nMaximumHP; } }

	public float MovementSpeed { get { return 0; } }
}
