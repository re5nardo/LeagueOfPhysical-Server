using UnityEngine;
using Entity;

public class ProjectileBuilder : EntityBuilder<ProjectileBuilder, Projectile, ProjectileCreationData>
{
    protected override ProjectileCreationData entityCreationData { get; set; } = new ProjectileCreationData();

    public ProjectileBuilder SetMasterDataId(int masterDataId)
    {
        entityCreationData.masterDataId = masterDataId;
        return this;
    }

	public ProjectileBuilder SetModelId(string modelId)
    {
        entityCreationData.modelId = modelId;
        return this;
    }

    public ProjectileBuilder SetProjectorId(int projectorId)
    {
        entityCreationData.projectorId = projectorId;
        return this;
    }

    public ProjectileBuilder SetLifespan(float lifespan)
    {
        entityCreationData.lifespan = lifespan;
        return this;
    }

	public ProjectileBuilder SetMovementSpeed(float movementSpeed)
	{
        entityCreationData.movementSpeed = movementSpeed;
        return this;
	}

    public override Projectile Build()
    {
        GameObject goProjectile = new GameObject(string.Format("Entity_{0}", entityCreationData.entityId));
        Projectile projectile = goProjectile.AddComponent<Projectile>();

        projectile.Initialize(entityCreationData);

        EntityManager.Instance.RegisterEntity(projectile);
		
		return projectile;
    }
}
