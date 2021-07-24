using System.Collections;
using System.Collections.Generic;
using Entity;
using UnityEngine;
using GameFramework;
using NetworkModel.PUN;

public class CharacterStatusData : MonoComponentBase
{
	private FirstStatus m_FirstStatus;
	private SecondStatus m_SecondStatus;

	private int m_nSelectableFirstStatusCount;

	public override void Initialize(params object[] param)
	{
		base.Initialize(param);

		m_FirstStatus = (FirstStatus)param[0];
		m_SecondStatus = (SecondStatus)param[1];
		m_nSelectableFirstStatusCount = (int)param[2];
	}

	public FirstStatus FirstStatus { get { return m_FirstStatus; } }
	public SecondStatus SecondStatus { get { return m_SecondStatus; } }

	public int CurrentHP
	{
		get { return m_SecondStatus.HP; }
		set { m_SecondStatus.HP = Mathf.Min(value, m_SecondStatus.MaximumHP); }
	}

	public int MaximumHP { get { return m_SecondStatus.MaximumHP; } }

	public int CurrentMP
	{
		get { return m_SecondStatus.MP; }
		set { m_SecondStatus.MP = Mathf.Min(value, m_SecondStatus.MaximumMP); }
	}

	public int MaximumMP { get { return m_SecondStatus.MaximumMP; } }

	public float MovementSpeed { get { return m_SecondStatus.MovementSpeed; } }

	public int STR
	{
		get { return m_FirstStatus.STR; }
		set
		{ 
			m_FirstStatus.STR = value;
			OnFirstStatusChange(FirstStatusElement.STR, value);
		}
	}

	public int DEX
	{
		get { return m_FirstStatus.DEX; }
		set
		{
			m_FirstStatus.DEX = value;
			OnFirstStatusChange(FirstStatusElement.DEX, value);
		}
	}

	public int CON
	{
		get { return m_FirstStatus.CON; }
		set
		{
			m_FirstStatus.CON = value;
			OnFirstStatusChange(FirstStatusElement.CON, value);
		}
	}

	public int INT
	{
		get { return m_FirstStatus.INT; }
		set
		{
			m_FirstStatus.INT = value;
			OnFirstStatusChange(FirstStatusElement.INT, value);
		}
	}

	public int WIS
	{
		get { return m_FirstStatus.WIS; }
		set
		{
			m_FirstStatus.WIS = value;
			OnFirstStatusChange(FirstStatusElement.WIS, value);
		}
	}

	public int CHA
	{
		get { return m_FirstStatus.CHA; }
		set
		{
			m_FirstStatus.CHA = value;
			OnFirstStatusChange(FirstStatusElement.CHA, value);
		}
	}

	public int SelectableFirstStatusCount { get { return m_nSelectableFirstStatusCount; } }

	public void IncreaseSelectableFirstStatusCount()
	{
		m_nSelectableFirstStatusCount++;

		RoomNetwork.Instance.Send(new SC_SelectableFirstStatusCount(m_nSelectableFirstStatusCount), PhotonHelper.GetActorID(Entity.EntityID));
	}

	public void DecreaseSelectableFirstStatusCount()
	{
		m_nSelectableFirstStatusCount--;

		RoomNetwork.Instance.Send(new SC_SelectableFirstStatusCount(m_nSelectableFirstStatusCount), PhotonHelper.GetActorID(Entity.EntityID));
	}

	public void IncreaseFirstStatus(FirstStatusElement element)
	{
		switch(element)
		{
			case FirstStatusElement.STR: STR++; break;
			case FirstStatusElement.DEX: DEX++; break;
			case FirstStatusElement.CON: CON++; break;
			case FirstStatusElement.INT: INT++; break;
			case FirstStatusElement.WIS: WIS++; break;
			case FirstStatusElement.CHA: CHA++; break;
			default: Debug.LogError("element is invalid! element : " + element); return;
		}
	}

	private void OnFirstStatusChange(FirstStatusElement element, int nValue)
	{
		MasterData.SecondStatus masterData = MasterDataManager.Instance.GetMasterData<MasterData.SecondStatus>((Entity as Character).MasterData.SecondStatusID);

		m_SecondStatus.Apply(m_FirstStatus, masterData);

		RoomNetwork.Instance.Send(new SC_CharacterStatusChange(m_FirstStatus, m_SecondStatus), PhotonHelper.GetActorID(Entity.EntityID));
	}
}
