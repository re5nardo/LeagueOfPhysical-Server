using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkModel.Mirror;

public class BehaviorParam
{
    public int masterDataId = -1;

    public BehaviorParam(int masterDataId)
    {
        this.masterDataId = masterDataId;
    }

    public virtual void Clear()
    {
        masterDataId = -1;
    }
}

class MoveBehaviorParam : BehaviorParam
{
    public Vector3 destination;

    public MoveBehaviorParam(int masterDataId, Vector3 destination) : base(masterDataId)
    {
        this.destination = destination;
    }

    public override void Clear()
    {
        base.Clear();

        destination = default;
    }
}

class RotationBehaviorParam : BehaviorParam
{
    public Vector3 direction;

    public RotationBehaviorParam(int masterDataId, Vector3 direction) : base(masterDataId)
    {
        this.direction = direction;
    }

    public override void Clear()
    {
        base.Clear();

        direction = default;
    }
}

class ContinuousPatrolBehaviorParam : BehaviorParam
{
    public Vector3 startPoint;
    public Vector3 halfwayPoint;
    public float timeOffset;

    public ContinuousPatrolBehaviorParam(int masterDataId, Vector3 startPoint, Vector3 halfwayPoint, float timeOffset) : base(masterDataId)
    {
        this.startPoint = startPoint;
        this.halfwayPoint = halfwayPoint;
        this.timeOffset = timeOffset;
    }

    public override void Clear()
    {
        base.Clear();

        startPoint = default;
        halfwayPoint = default;
        timeOffset = default;
    }
}

class ContinuousRotationBehaviorParam : BehaviorParam
{
    public Vector3 startRotation;
    public float timeOffset;

    public ContinuousRotationBehaviorParam(int masterDataId, Vector3 startRotation, float timeOffset) : base(masterDataId)
    {
        this.startRotation = startRotation;
        this.timeOffset = timeOffset;
    }

    public override void Clear()
    {
        base.Clear();

        startRotation = default;
        timeOffset = default;
    }
}

class AttackBehaviorParam : BehaviorParam
{
    public SkillInputData skillInputData;

    public AttackBehaviorParam(int masterDataId, SkillInputData skillInputData) : base(masterDataId)
    {
        this.skillInputData = skillInputData;
    }

    public override void Clear()
    {
        base.Clear();

        skillInputData = default;
    }
}
