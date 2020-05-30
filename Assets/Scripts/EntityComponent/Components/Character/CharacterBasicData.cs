using UnityEngine;

public class CharacterBasicData : EntityBasicData
{
	private int m_nMasterDataID = -1;

	public override void Initialize(params object[] param)
	{
		base.Initialize(param[1]);

		m_nMasterDataID = (int)param[0];
	}

	public int MasterDataID { get { return m_nMasterDataID; } }
}
