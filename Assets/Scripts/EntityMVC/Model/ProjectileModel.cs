using UnityEngine;
using Entity;
using EntityCommand;
using GameFramework;
using System.Collections.Generic;

public class ProjectileModel : BasicModel
{
	private int m_nMasterDataID = -1;
	private int m_nProjectorID = -1;
	private float m_fMovementSpeed = 0f;

    private HashSet<int> attackList = new HashSet<int>();

	public override void Initialize(params object[] param)
	{
		base.Initialize(param[2]);

		m_nProjectorID = (int)param[0];
		m_nMasterDataID = (int)param[1];
		m_fMovementSpeed = (float)param[3];
	}

	public override void OnCommand(ICommand command)
	{
		base.OnCommand(command);

		if (command is ModelTriggerEnter)
		{
			ModelTriggerEnter cmd = command as ModelTriggerEnter;

			IEntity target = EntityManager.Instance.GetEntity(cmd.targetEntityID);
            if (target == null)
            {
                //  Already destroyed
                return;
            }
			else if(target is Character)
			{
				if (target.EntityID == ProjectorID || !(target as Character).IsAlive)
				{
					return;
				}
			}
			else if (target is GameItem)
			{
				if (!(target as GameItem).IsAlive)
				{
					return;
				}
			}
            else if (target is Projectile)
            {
                return;
            }

            if (!attackList.Contains(target.EntityID))
            {
                LOP.Game.Current.AttackEntity(ProjectorID, target.EntityID);

                LOP.Game.Current.DestroyEntity(Entity.EntityID);

                attackList.Add(target.EntityID);
            }
        }
	}

	public int MasterDataID { get { return m_nMasterDataID; } }

	public int ProjectorID { get { return m_nProjectorID; } }

	public float MovementSpeed { get { return m_fMovementSpeed; } }
}
