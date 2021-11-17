using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StateAttribute
{
    public string name;
}

[Serializable]
public class SubClassStateAttribute : StateAttribute
{
    public string[] classNames;
}
