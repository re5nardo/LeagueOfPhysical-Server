using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EnvironmentSettings", menuName = "ScriptableObjects/EnvironmentSettings")]
public class EnvironmentSettings : ScriptableObject
{
    [Serializable]
    public class BaseURLSetting
    {
        public string scheme;
        public string host;
    }

    [SerializeField] private BaseURLSetting roomServerSetting;

    private static EnvironmentSettings instance;
    public static EnvironmentSettings active
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<EnvironmentSettings>("ScriptableObject/EnvironmentSettings/EnvironmentSettings_Dev");
            }
            return instance;
        }
    }

    public string roomBaseURL => $"{roomServerSetting.scheme}://{roomServerSetting.host}";
}
