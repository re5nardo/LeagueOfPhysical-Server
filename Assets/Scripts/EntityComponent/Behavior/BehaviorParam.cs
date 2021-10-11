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
}

public class MoveBehaviorParam : BehaviorParam
{
    public Vector3 destination;

    public MoveBehaviorParam(int masterDataId, Vector3 destination) : base(masterDataId)
    {
        this.destination = destination;
    }
}

public class JumpBehaviorParam : BehaviorParam
{
    public float normalizedPower;
    public Vector3 direction;

    public JumpBehaviorParam(int masterDataId, float normalizedPower, Vector3 direction) : base(masterDataId)
    {
        this.normalizedPower = normalizedPower;
        this.direction = direction;
    }
}

public class RotationBehaviorParam : BehaviorParam
{
    public Vector3 direction;

    public RotationBehaviorParam(int masterDataId, Vector3 direction) : base(masterDataId)
    {
        this.direction = direction;
    }
}

public class ContinuousPatrolBehaviorParam : BehaviorParam
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
}

public class ContinuousRotationBehaviorParam : BehaviorParam
{
    public Vector3 startRotation;
    public float timeOffset;

    public ContinuousRotationBehaviorParam(int masterDataId, Vector3 startRotation, float timeOffset) : base(masterDataId)
    {
        this.startRotation = startRotation;
        this.timeOffset = timeOffset;
    }
}

public class AttackBehaviorParam : BehaviorParam
{
    public SkillInputData skillInputData;

    public AttackBehaviorParam(int masterDataId, SkillInputData skillInputData) : base(masterDataId)
    {
        this.skillInputData = skillInputData;
    }
}

public class ContinuousMoveBehaviorParam : BehaviorParam
{
    public Vector3 direction;

    public ContinuousMoveBehaviorParam(int masterDataId, Vector3 direction) : base(masterDataId)
    {
        this.direction = direction;
    }
}
