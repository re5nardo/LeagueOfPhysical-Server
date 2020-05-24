using GameFramework;

namespace EntityCommand
{
    public class PositionChanged : ICommand
    {
    }

    public class RotationChanged : ICommand
    {
    }

    public class ModelTriggerEnter : ICommand
    {
        public int targetEntityID;

        public ModelTriggerEnter(int targetEntityID)
        {
            this.targetEntityID = targetEntityID;
        }
    }

    public class ModelChanged : ICommand
    {
        public string name;

        public ModelChanged(string name)
        {
            this.name = name;
        }
    }

    public class AnimatorSetTrigger : ICommand
    {
        public string name;

        public AnimatorSetTrigger(string name)
        {
            this.name = name;
        }
    }

    public class AnimatorSetFloat : ICommand
    {
        public string name;
        public float value;

        public AnimatorSetFloat(string name, float value)
        {
            this.name = name;
            this.value = value;
        }
    }

    public class AnimatorSetBool : ICommand
    {
        public string name;
        public bool value;

        public AnimatorSetBool(string name, bool value)
        {
            this.name = name;
            this.value = value;
        }
    }

    public class Destroying : ICommand
    {
    }
}
