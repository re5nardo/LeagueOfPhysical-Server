using UnityEngine;
using Entity;

public class CharacterBuilder
{
    private int m_nMasterDataID = -1;
    private Vector3 m_vec3Position = Vector3.zero;
    private Vector3 m_vec3Rotation = Vector3.zero;
	private Vector3 m_vec3Velocity = Vector3.zero;
	private Vector3 m_vec3AngularVelocity = Vector3.zero;
	private string m_strModelPath = "";
    private FirstStatus m_FirstStatus = default;
	private SecondStatus m_SecondStatus = default;
	private int m_nSelectableFirstStatusCount = default;
    private EntityRole m_EntityRole = default;

    public CharacterBuilder SetMasterDataID(int nMasterDataID)
    {
        m_nMasterDataID = nMasterDataID;
        return this;
    }

    public CharacterBuilder SetPosition(Vector3 vec3Position)
    {
        m_vec3Position = vec3Position;
        return this;
    }
	
    public CharacterBuilder SetRotation(Vector3 vec3Rotation)
    {
        m_vec3Rotation = vec3Rotation;
        return this;
    }

	public CharacterBuilder SetVelocity(Vector3 vec3Velocity)
	{
		m_vec3Velocity = vec3Velocity;
		return this;
	}

	public CharacterBuilder SetAngularVelocity(Vector3 vec3AngularVelocity)
	{
		m_vec3AngularVelocity = vec3AngularVelocity;
		return this;
	}

	public CharacterBuilder SetModelPath(string strModelPath)
    {
        m_strModelPath = strModelPath;
        return this;
    }

    public CharacterBuilder SetFirstStatus(FirstStatus firstStatus)
    {
		m_FirstStatus = firstStatus;
        return this;
    }

	public CharacterBuilder SetSecondStatus(SecondStatus secondStatus)
	{
		m_SecondStatus = secondStatus;
		return this;
	}

	public CharacterBuilder SetSelectableFirstStatusCount(int nSelectableFirstStatusCount)
	{
		m_nSelectableFirstStatusCount = nSelectableFirstStatusCount;
		return this;
	}

    public CharacterBuilder SetEntityRole(EntityRole entityRole)
    {
        m_EntityRole = entityRole;
        return this;
    }

    public Character Build()
    {
        GameObject goCharacter = new GameObject();
        Character character = goCharacter.AddComponent<Character>();

		character.Initialize(m_nMasterDataID, m_strModelPath, m_FirstStatus, m_SecondStatus, m_nSelectableFirstStatusCount, m_EntityRole);
		character.Position = m_vec3Position;
		character.Rotation = m_vec3Rotation;
		character.Velocity = m_vec3Velocity;
		character.AngularVelocity = m_vec3AngularVelocity;

        goCharacter.name = string.Format("Entity_{0}", character.EntityID);

        EntityManager.Instance.RegisterEntity(character);

		return character;
    }

    public void Clear()
    {
		m_nMasterDataID = -1;
		m_vec3Position = Vector3.zero;
        m_vec3Rotation = Vector3.zero;
		m_vec3Velocity = Vector3.zero;
		m_vec3AngularVelocity = Vector3.zero;
		m_strModelPath = "";
		m_FirstStatus = default;
		m_SecondStatus = default;
		m_nSelectableFirstStatusCount = default;
        m_EntityRole = default;
    }
}
