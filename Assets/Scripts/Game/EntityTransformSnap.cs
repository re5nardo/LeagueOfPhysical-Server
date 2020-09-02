using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class EntityTransformSnap
{
    public int WaitingInterval = 3;  //  최소 대기 시간(tick) (이 시간 이하로는 다시 send 불가)

    private int entityID = -1;
    private IEntity entity = null;
    private IEntity Entity
    {
        get
        {
            if (entity == null)
            {
                entity = Entities.Get(entityID);
            }
            return entity;
        }
    }

    private Vector3 lastPosition;
    private Vector3 lastRotation;
    private Vector3 lastVelocity;
    private Vector3 lastAngularVelocity;

    public int LastSendTick = -1;

    public bool HasChanged
    {
        get
        {
            return (lastPosition != Entity.Position || lastRotation != Entity.Rotation || lastVelocity != Entity.Velocity || lastAngularVelocity != Entity.AngularVelocity);
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

    public bool HasVelocityChanged
    {
        get
        {
            return (lastVelocity != Entity.Velocity || lastAngularVelocity != Entity.AngularVelocity);
        }
        set
        {
            if (value == false)
            {
                lastVelocity = Entity.Velocity;
                lastAngularVelocity = Entity.AngularVelocity;
            }
        }
    }

    public EntityTransformSnap(int entityID)
    {
        this.entityID = entityID;

        //  Appear protocol에서 transform 정보가 전송되므로 현재 기준으로 변수 값들 세팅
        lastPosition = Entity.Position;
        lastRotation = Entity.Rotation;
        lastVelocity = Entity.Velocity;
        lastAngularVelocity = Entity.AngularVelocity;

        LastSendTick = Game.Current.CurrentTick;
    }
}
