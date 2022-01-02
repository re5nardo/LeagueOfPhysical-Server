using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using GameFramework;

namespace LOP
{
    public class Application
    {
        public static bool IsApplicationQuitting;
        public static bool IsInitialized { get; private set; }

        public static string IP { get; private set; }
        public static string UserId => "server";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod()
        {
            GlobalMonoBehavior.StartCoroutine(Initialize());
        }

        private static IEnumerator Initialize()
        {
            //  Target FrameRate
            UnityEngine.Application.targetFrameRate = 60;

            //  BehaviorDesigner.Runtime.BehaviorManager
            var behaviorManager = new GameObject("BehaviorManager").AddComponent<BehaviorDesigner.Runtime.BehaviorManager>();
            behaviorManager.UpdateInterval = BehaviorDesigner.Runtime.UpdateIntervalType.Manual;
            UnityEngine.Object.DontDestroyOnLoad(behaviorManager);

            MonoSingletonBase.condition = () => !IsApplicationQuitting;

            MasterDataManager.Instantiate();

            yield return GlobalMonoBehavior.StartCoroutine(GetPublicIP());

            IsInitialized = true;
        }

        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }

        private static IEnumerator GetPublicIP()
        {
            using (var www = UnityWebRequest.Get("http://ipinfo.io/ip"))
            {
                yield return www.SendWebRequest();

                IP = Regex.Replace(www.downloadHandler.text, @"[^0-9.]", "");
            }
        }
    }
}
