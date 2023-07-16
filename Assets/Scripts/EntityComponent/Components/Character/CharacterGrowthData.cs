using GameFramework;
using Entity;
using GameEvent;

public class CharacterGrowthData : LOPEntityComponentBase
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
                if (Entity.EntityRole == EntityRole.Player)
                {
                    if (GameIdMap.TryGetConnectionIdByEntityId(Entity.EntityId, out var connectionId))
                    {
                        LOP.Game.Current.GameEventManager.Send(new EntityLevelUp(Entity.EntityId, value), connectionId);
                    }
                }
            }
        }
    }

	public int Exp { get; set; }
}
