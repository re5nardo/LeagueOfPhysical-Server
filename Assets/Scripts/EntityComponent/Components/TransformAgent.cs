using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class TransformAgent : MonoComponentBase
{
    public int WaitingInterval = 3;  //  최소 대기 시간(tick) (이 시간 이하로는 다시 send 불가)

    public Vector3 LastPosition;
    public Vector3 LastRotation;
    public Vector3 LastVelocity;
    public Vector3 LastAngularVelocity;

    public int LastSendTick = -1;

    public bool HasChanged
    {
        get
        {
            return  LastPosition != Entity.Position ||
                    LastRotation != Entity.Rotation ||
                    LastVelocity != Entity.Velocity ||
                    LastAngularVelocity != Entity.AngularVelocity;
        }
        set
        {
            if (value == false)
            {
                LastPosition = Entity.Position;
                LastRotation = Entity.Rotation;
                LastVelocity = Entity.Velocity;
                LastAngularVelocity = Entity.AngularVelocity;
            }
        }
    }

    public override void OnAttached(IEntity entity)
    {
        base.OnAttached(entity);

        LastPosition = Entity.Position;
        LastRotation = Entity.Rotation;
        LastVelocity = Entity.Velocity;
        LastAngularVelocity = Entity.AngularVelocity;

        LastSendTick = Game.Current.CurrentTick;
    }
}
