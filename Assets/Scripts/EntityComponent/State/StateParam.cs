using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateParam
{
    public int masterDataId = -1;

    public StateParam(int masterDataId)
    {
        this.masterDataId = masterDataId;
    }
}

public class BasicStateParam : StateParam
{
    public float lifespan;

    public BasicStateParam(int masterDataId, float lifespan) : base(masterDataId)
    {
        this.lifespan = lifespan;
    }
}
