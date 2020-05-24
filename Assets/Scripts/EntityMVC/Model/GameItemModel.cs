using UnityEngine;
using Entity;
using EntityCommand;
using GameFramework;

public class GameItemModel : BasicModel
{
	private int m_nMasterDataID = -1;
	private int m_nHP = 0;
	private int m_nMaximumHP = 0;

	public override void Initialize(params object[] param)
	{
		base.Initialize(param[1]);

		m_nMasterDataID = (int)param[0];

		var masterData = MasterDataManager.Instance.GetMasterData<MasterData.GameItem>(m_nMasterDataID);
		m_nHP = m_nMaximumHP = masterData.HP;
	}

	public override void OnCommand(ICommand command)
	{
		base.OnCommand(command);

		if (command is ModelTriggerEnter)
		{
			ModelTriggerEnter cmd = command as ModelTriggerEnter;

			Character target = EntityManager.Instance.GetEntity(cmd.targetEntityID) as Character;
			if (target == null || !target.IsAlive)
				return;

            LOP.Game.Current.EntityGetGameItem(target.EntityID, Entity.EntityID);
		}
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
