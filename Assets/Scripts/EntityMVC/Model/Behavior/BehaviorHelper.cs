using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorHelper
{
    public static void BehaviorDestroyer(Behavior.BehaviorBase behavior)
    {
		behavior.Entity.DetachComponent(behavior);

        Object.Destroy(behavior);
    }
}
