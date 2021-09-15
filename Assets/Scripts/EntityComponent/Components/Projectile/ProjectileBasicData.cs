using UnityEngine;
using EntityMessage;

public class ProjectileBasicData : EntityBasicData
{
    public int MasterDataId { get; private set; } = -1;

    private string modelId;
    public string ModelId
    {
        get => modelId;
        private set
        {
            modelId = value;
            Entity.MessageBroker.Publish(new ModelChanged(value));
        }
    }

    public int ProjectorId { get; private set; } = -1;

    public float MovementSpeed { get; private set; }

	public override void Initialize(EntityCreationData entityCreationData)
	{
		base.Initialize(entityCreationData);

        ProjectileCreationData projectileCreationData = entityCreationData as ProjectileCreationData;

        MasterDataId = projectileCreationData.masterDataId;
        ModelId = projectileCreationData.modelId;
        ProjectorId = projectileCreationData.projectorId;
        MovementSpeed = projectileCreationData.movementSpeed;
    }
}
