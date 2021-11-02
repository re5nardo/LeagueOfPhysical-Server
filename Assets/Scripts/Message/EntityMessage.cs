using UnityEngine;

namespace EntityMessage
{
    public struct PositionChanged
    {
    }

    public struct RotationChanged
    {
    }

    public struct VelocityChanged
    {
    }

    public struct AngularVelocityChanged
    {
    }

    public struct ModelTriggerEnter
    {
        public int entityId;
        public Collider self;
        public Collider other;

        public ModelTriggerEnter(int entityId, Collider self, Collider other)
        {
            this.entityId = entityId;
            this.self = self;
            this.other = other;
        }
    }

    public struct ModelChanged
    {
        public string name;

        public ModelChanged(string name)
        {
            this.name = name;
        }
    }

    public struct AnimatorSetTrigger
    {
        public string name;

        public AnimatorSetTrigger(string name)
        {
            this.name = name;
        }
    }

    public struct AnimatorSetFloat
    {
        public string name;
        public float value;

        public AnimatorSetFloat(string name, float value)
        {
            this.name = name;
            this.value = value;
        }
    }

    public struct AnimatorSetBool
    {
        public string name;
        public bool value;

        public AnimatorSetBool(string name, bool value)
        {
            this.name = name;
            this.value = value;
        }
    }

    public struct Destroying
    {
    }
}
