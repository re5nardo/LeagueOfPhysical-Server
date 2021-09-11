using UnityEngine;
using Entity;
using EntityCommand;
using GameFramework;
using System.Collections.Generic;

public class ProjectileBasicData : EntityBasicData
{
    public string ModelId { get; private set; }
    private int m_nMasterDataID = -1;
	private int m_nProjectorID = -1;
	private float m_fMovementSpeed = 0f;

	public override void Initialize(EntityCreationData entityCreationData)
	{
		base.Initialize(entityCreationData);

        ProjectileCreationData projectileCreationData = entityCreationData as ProjectileCreationData;

        m_nMasterDataID = projectileCreationData.masterDataId;
        m_nProjectorID = projectileCreationData.projectorId;
        m_fMovementSpeed = projectileCreationData.movementSpeed;

        ModelId = projectileCreationData.modelId;

        Entity.SendCommandToViews(new ModelChanged(ModelId));
    }

	public int MasterDataID { get { return m_nMasterDataID; } }

	public int ProjectorID { get { return m_nProjectorID; } }

	public float MovementSpeed { get { return m_fMovementSpeed; } }
}
