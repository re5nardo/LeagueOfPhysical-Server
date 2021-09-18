
namespace TickMessage
{
    public struct EarlyTick
    {
        public int tick;

        public EarlyTick(int tick)
        {
            this.tick = tick;
        }
    }

    public struct Tick
    {
        public int tick;

        public Tick(int tick)
        {
            this.tick = tick;
        }
    }

    public struct LateTick
    {
        public int tick;

        public LateTick(int tick)
        {
            this.tick = tick;
        }
    }

    public struct EarlyTickEnd
    {
        public int tick;

        public EarlyTickEnd(int tick)
        {
            this.tick = tick;
        }
    }

    public struct TickEnd
    {
        public int tick;

        public TickEnd(int tick)
        {
            this.tick = tick;
        }
    }

    public struct LateTickEnd
    {
        public int tick;

        public LateTickEnd(int tick)
        {
            this.tick = tick;
        }
    }

    public struct BeforePhysicsSimulation
    {
        public int tick;

        public BeforePhysicsSimulation(int tick)
        {
            this.tick = tick;
        }
    }

    public struct AfterPhysicsSimulation
    {
        public int tick;

        public AfterPhysicsSimulation(int tick)
        {
            this.tick = tick;
        }
    }
}
