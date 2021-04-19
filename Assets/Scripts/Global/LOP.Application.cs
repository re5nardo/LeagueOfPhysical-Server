using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOP
{
    public class Application
    {
        public static bool IsApplicationQuitting => GlobalMonoBehavior.Instance.IsApplicationQuitting;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod()
        {
            Initialize();
        }

        private static void Initialize()
        {
            //  Target FrameRate
            UnityEngine.Application.targetFrameRate = 30;

            //  BehaviorDesigner.Runtime.BehaviorManager
            var behaviorManager = new GameObject("BehaviorManager").AddComponent<BehaviorDesigner.Runtime.BehaviorManager>();
            behaviorManager.UpdateInterval = BehaviorDesigner.Runtime.UpdateIntervalType.Manual;
            UnityEngine.Object.DontDestroyOnLoad(behaviorManager);

            //  PhotonType Register
            PhotonTypeRegister.Register();

            MasterDataManager.Instantiate();
        }

        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}
