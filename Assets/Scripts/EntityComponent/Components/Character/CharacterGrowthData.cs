using GameFramework;
using Entity;
using GameEvent;

public class CharacterGrowthData : ComponentBase
{
	private int m_nLevel;
	public int m_nExp;

    public int Level
    {
        get
        {
            return m_nLevel;
        }
        set
        {
            int pre = m_nLevel;

            m_nLevel = value;

            if (pre < value)
            {
                if ((Entity as MonoEntityBase).EntityRole == EntityRole.Player)
                {
                    LOP.Game.Current.GameEventManager.Send(new EntityLevelUp(Entity.EntityID, value), PhotonHelper.GetPhotonPlayer(Entity.EntityID).ID);
                }

                GamePubSubService.Publish(GameMessageKey.LevelUp, new object[] { Entity.EntityID, value });
            }
        }
    }

    public void Initialize(int level, int exp)
    {
        m_nLevel = level;
        m_nExp = exp;
    }
}
