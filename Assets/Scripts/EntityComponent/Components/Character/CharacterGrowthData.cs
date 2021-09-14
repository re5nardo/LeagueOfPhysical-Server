using GameFramework;
using Entity;
using GameEvent;

public class CharacterGrowthData : EntityComponentBase
{
    private int level = 1;
    public int Level
    {
        get => level;
        set
        {
            var levelUp = level < value;

            level = value;

            if (levelUp)
            {
                if ((Entity as LOPMonoEntityBase).EntityRole == EntityRole.Player)
                {
                    if (IDMap.TryGetConnectionIdByEntityId(Entity.EntityID, out var connectionId))
                    {
                        LOP.Game.Current.GameEventManager.Send(new EntityLevelUp(Entity.EntityID, value), connectionId);
                    }
                }

                GamePubSubService.Publish(GameMessageKey.LevelUp, new object[] { Entity.EntityID, value });
            }
        }
    }

	public int Exp { get; set; }
}
