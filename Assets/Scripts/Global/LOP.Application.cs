﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOP
{
    public class Application
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod()
        {
            Initialize();
        }

        private static void Initialize()
        {
            //  Target FrameRate
            UnityEngine.Application.targetFrameRate = 60;

            //  BehaviorDesigner.Runtime.BehaviorManager
            GameObject goBehaviorManager = new GameObject("BehaviorManager");
            UnityEngine.Object.DontDestroyOnLoad(goBehaviorManager);

            BehaviorDesigner.Runtime.BehaviorManager behaviorManager = goBehaviorManager.AddComponent<BehaviorDesigner.Runtime.BehaviorManager>();
            behaviorManager.UpdateInterval = BehaviorDesigner.Runtime.UpdateIntervalType.Manual;
        }
    }
}
