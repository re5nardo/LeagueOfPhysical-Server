using UnityEngine;
using Entity;
using EntityCommand;
using GameFramework;
using System.Collections.Generic;

public class ProjectileBasicData : EntityBasicData
{
	private int m_nMasterDataID = -1;
	private int m_nProjectorID = -1;
	private float m_fMovementSpeed = 0f;

	public override void Initialize(params object[] param)
	{
		base.Initialize(param[2]);

		m_nProjectorID = (int)param[0];
		m_nMasterDataID = (int)param[1];
		m_fMovementSpeed = (float)param[3];
	}

	public int MasterDataID { get { return m_nMasterDataID; } }

	public int ProjectorID { get { return m_nProjectorID; } }

	public float MovementSpeed { get { return m_fMovementSpeed; } }
}
