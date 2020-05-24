using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOP
{
    public class Application
    {
        private static GlobalMonoBehavior globalMonoBehavior = null;

        public static bool IsApplicationQuitting { get { return globalMonoBehavior.IsApplicationQuitting; } }

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

            //  Global GameObject
            globalMonoBehavior = new GameObject("GlobalGameObject").AddComponent<GlobalMonoBehavior>();
            UnityEngine.Object.DontDestroyOnLoad(globalMonoBehavior.gameObject);

            //  PhotonType Register
            PhotonTypeRegister.Register();
        }
    }
}
