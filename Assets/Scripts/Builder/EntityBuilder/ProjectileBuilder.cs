using UnityEngine;
using Entity;

public class ProjectileBuilder
{
    private int m_nMasterDataID = -1;
    private Vector3 m_vec3Position = Vector3.zero;
    private Vector3 m_vec3Rotation = Vector3.zero;
    private Vector3 m_vec3Velocity = Vector3.zero;
	private Vector3 m_vec3AngularVelocity = Vector3.zero;
	private string m_strModelPath = "";
    private int m_nProjectorID = -1;
    private float m_fLifespan = 0f;
	private float m_fMovementSpeed = 0f;
    private EntityRole m_EntityRole = default;

    public ProjectileBuilder SetMasterDataID(int nMasterDataID)
    {
        m_nMasterDataID = nMasterDataID;
        return this;
    }

    public ProjectileBuilder SetPosition(Vector3 vec3Position)
    {
        m_vec3Position = vec3Position;
        return this;
    }

    public ProjectileBuilder SetRotation(Vector3 vec3Rotation)
    {
        m_vec3Rotation = vec3Rotation;
        return this;
    }

    public ProjectileBuilder SetVelocity(Vector3 vec3Velocity)
    {
        m_vec3Velocity = vec3Velocity;
        return this;
    }

	public ProjectileBuilder SetAngularVelocity(Vector3 vec3AngularVelocity)
	{
		m_vec3AngularVelocity = vec3AngularVelocity;
		return this;
	}

	public ProjectileBuilder SetModelPath(string strModelPath)
    {
        m_strModelPath = strModelPath;
        return this;
    }

    public ProjectileBuilder SetProjectorID(int nProjectorID)
    {
        m_nProjectorID = nProjectorID;
        return this;
    }

    public ProjectileBuilder SetLifespan(float fLifespan)
    {
        m_fLifespan = fLifespan;
        return this;
    }

	public ProjectileBuilder SetMovementSpeed(float fMovementSpeed)
	{
		m_fMovementSpeed = fMovementSpeed;
		return this;
	}

    public ProjectileBuilder SetEntityRole(EntityRole entityRole)
    {
        m_EntityRole = entityRole;
        return this;
    }

    public Projectile Build()
    {
        GameObject goProjectile = new GameObject();
        Projectile projectile = goProjectile.AddComponent<Projectile>();

        projectile.Initialize(m_nProjectorID, m_nMasterDataID, m_strModelPath, m_fMovementSpeed, m_EntityRole);
		projectile.Position = m_vec3Position;
		projectile.Rotation = m_vec3Rotation;
		projectile.Velocity = m_vec3Velocity;
		projectile.AngularVelocity = m_vec3AngularVelocity;

        StateController stateController = projectile.GetComponent<StateController>();
        stateController.StartState(MasterDataDefine.StateID.EntitySelfDestroy, m_fLifespan);

        goProjectile.name = string.Format("Entity_{0}", projectile.EntityID);

        EntityManager.Instance.RegisterEntity(projectile);
		
		return projectile;
    }

    public void Clear()
    {
		m_nMasterDataID = -1;
		m_vec3Position = Vector3.zero;
        m_vec3Rotation = Vector3.zero;
		m_vec3Velocity = Vector3.zero;
		m_vec3AngularVelocity = Vector3.zero;
		m_strModelPath = "";
		m_nProjectorID = -1;
		m_fLifespan = 0f;
		m_fMovementSpeed = 0f;
        m_EntityRole = default;
    }
}
