using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class TransformAgent : MonoComponentBase
{
    public int WaitingInterval = 0;  //  최소 대기 시간(tick) (이 시간 이하로는 다시 send 불가)
  
    private Vector3 lastPosition;
    private Vector3 lastRotation;
    private Vector3 lastVelocity;
    private Vector3 lastAngularVelocity;

    public int LastSendTick = -1;

    public bool HasChanged
    {
        get
        {
            return lastPosition != Entity.Position ||
                    lastRotation != Entity.Rotation ||
                    lastVelocity != Entity.Velocity ||
                    lastAngularVelocity != Entity.AngularVelocity;
        }
        set
        {
            if (value == false)
            {
                lastPosition = Entity.Position;
                lastRotation = Entity.Rotation;
                lastVelocity = Entity.Velocity;
                lastAngularVelocity = Entity.AngularVelocity;
            }
        }
    }

    public override void OnAttached(IEntity entity)
    {
        base.OnAttached(entity);

        lastPosition = Entity.Position;
        lastRotation = Entity.Rotation;
        lastVelocity = Entity.Velocity;
        lastAngularVelocity = Entity.AngularVelocity;

        LastSendTick = Game.Current.CurrentTick;
    }
}
