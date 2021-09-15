
namespace EntityMessage
{
    public class PositionChanged
    {
    }

    public class RotationChanged
    {
    }

    public class VelocityChanged
    {
    }

    public class AngularVelocityChanged
    {
    }

    public class ModelTriggerEnter
    {
        public int targetEntityID;

        public ModelTriggerEnter(int targetEntityID)
        {
            this.targetEntityID = targetEntityID;
        }
    }

    public class ModelChanged
    {
        public string name;

        public ModelChanged(string name)
        {
            this.name = name;
        }
    }

    public class AnimatorSetTrigger
    {
        public string name;

        public AnimatorSetTrigger(string name)
        {
            this.name = name;
        }
    }

    public class AnimatorSetFloat
    {
        public string name;
        public float value;

        public AnimatorSetFloat(string name, float value)
        {
            this.name = name;
            this.value = value;
        }
    }

    public class AnimatorSetBool
    {
        public string name;
        public bool value;

        public AnimatorSetBool(string name, bool value)
        {
            this.name = name;
            this.value = value;
        }
    }

    public class Destroying
    {
    }
}
