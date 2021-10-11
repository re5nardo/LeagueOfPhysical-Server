
namespace Define
{
    namespace MasterData
    {
        public class GameItemID
        {
            public const int RED_POTION = 0;
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
            public const int JUMP = 3;
            public const int DIE = 4;

            public const int CONTINUOUS_PATROL = 10000;
            public const int CONTINUOUS_ROTATION = 10001;
            public const int CONTINUOUS_MOVE = 10002;
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
}
