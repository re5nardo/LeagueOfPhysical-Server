
namespace MasterDataDefine
{
    public class GameItem
    {
		public const int EXP_POTION = 9;

        public const int RED_POTION = 0;
        public const int ORANGE_POTION = 11;
        public const int WHITE_POTION = 12;

		public const int TREASURE_BOX = 2;
	}

	public class CharacterID
    {
        public const int KNIGHT = 0;
        public const int NECROMANCER = 1;
        public const int ARCHER = 2;
		public const int EVELYNN = 3;
		public const int GAREN = 4;
		public const int VEIGAR = 5;
		public const int ASHE = 6;
		public const int SORAKA = 7;
		public const int MALPHITE = 8;
	}

    public class BehaviorID
    {
        public const int IDLE = 0;
        public const int MOVE = 1;
        public const int ROTATION = 2;
        public const int DIE = 3;
    }

    public class StateID
    {
        public const int Invincible = 0;
        public const int EntitySelfDestroy = 1;
    }

    public class SkillID
    {
        public const int PLASMA_FISSION = 3;
    }

    public class ProjectileID
    {
        public const int PLASMA_1 = 1;
    }
}