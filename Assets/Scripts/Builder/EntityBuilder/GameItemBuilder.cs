using UnityEngine;
using Entity;

public class GameItemBuilder
{
	private int m_nMasterDataID = -1;
	private Vector3 m_vec3Position = Vector3.zero;
	private Vector3 m_vec3Rotation = Vector3.zero;
	private Vector3 m_vec3Velocity = Vector3.zero;
	private Vector3 m_vec3AngularVelocity = Vector3.zero;
	private string m_strModelPath = "";
	private float m_fLifespan = -1;
    private EntityRole m_EntityRole = default;

	public GameItemBuilder SetMasterDataID(int nMasterDataID)
	{
		m_nMasterDataID = nMasterDataID;
		return this;
	}

	public GameItemBuilder SetPosition(Vector3 vec3Position)
	{
		m_vec3Position = vec3Position;
		return this;
	}

	public GameItemBuilder SetRotation(Vector3 vec3Rotation)
	{
		m_vec3Rotation = vec3Rotation;
		return this;
	}

	public GameItemBuilder SetVelocity(Vector3 vec3Velocity)
	{
		m_vec3Velocity = vec3Velocity;
		return this;
	}

	public GameItemBuilder SetAngularVelocity(Vector3 vec3AngularVelocity)
	{
		m_vec3AngularVelocity = vec3AngularVelocity;
		return this;
	}

	public GameItemBuilder SetModelPath(string strModelPath)
	{
		m_strModelPath = strModelPath;
		return this;
	}

	public GameItemBuilder SetLifespan(float fLifespan)
	{
		m_fLifespan = fLifespan;
		return this;
	}

    public GameItemBuilder SetEntityRole(EntityRole entityRole)
    {
        m_EntityRole = entityRole;
        return this;
    }

    public GameItem Build()
	{
		GameObject goGameItem = new GameObject();
		GameItem gameItem = goGameItem.AddComponent<GameItem>();

		gameItem.Initialize(m_nMasterDataID, m_strModelPath, m_EntityRole);
		gameItem.Position = m_vec3Position;
		gameItem.Rotation = m_vec3Rotation;
		gameItem.Velocity = m_vec3Velocity;
		gameItem.AngularVelocity = m_vec3AngularVelocity;

        StateController stateController = gameItem.GetComponent<StateController>();
        stateController.StartState(MasterDataDefine.StateID.EntitySelfDestroy, m_fLifespan);

        gameItem.name = string.Format("Entity_{0}", gameItem.EntityID);

        EntityManager.Instance.RegisterEntity(gameItem);
		
		return gameItem;
	}

	public void Clear()
	{
		m_nMasterDataID = -1;
		m_vec3Position = Vector3.zero;
		m_vec3Rotation = Vector3.zero;
		m_vec3Velocity = Vector3.zero;
		m_vec3AngularVelocity = Vector3.zero;
		m_strModelPath = "";
		m_fLifespan = -1;
        m_EntityRole = default;
    }
}
