using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class TransformAgent : MonoComponentBase, ITickable
{
    public int WaitingInterval = 3;  //  최소 대기 시간(tick) (이 시간 이하로는 다시 send 불가)
  
    private Vector3 lastPosition;
    private Vector3 lastRotation;
    private Vector3 lastVelocity;
    private Vector3 lastAngularVelocity;

    public int LastSendTick = -1;

    private bool hasChanged = false;
    public bool HasChanged
    {
        get => hasChanged;
        set
        {
            if (value == false)
            {
                lastPosition = Entity.Position;
                lastRotation = Entity.Rotation;
                lastVelocity = Entity.Velocity;
                lastAngularVelocity = Entity.AngularVelocity;

                HasVelocityChanged = false;
            }

            hasChanged = value;
        }
    }

    public bool HasVelocityChanged { get; private set; }

    public override void OnAttached(IEntity entity)
    {
        base.OnAttached(entity);

        lastPosition = Entity.Position;
        lastRotation = Entity.Rotation;
        lastVelocity = Entity.Velocity;
        lastAngularVelocity = Entity.AngularVelocity;

        LastSendTick = Game.Current.CurrentTick;
    }

    public void Tick(int tick)
    {
        if (lastVelocity != Entity.Velocity || lastAngularVelocity != Entity.AngularVelocity)
        {
            HasChanged = HasVelocityChanged = true;
        }
        else if (lastPosition != Entity.Position || lastRotation != Entity.Rotation)
        {
            HasChanged = true;
        }
    }
}
