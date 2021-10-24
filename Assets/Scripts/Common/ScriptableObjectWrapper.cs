using UnityEngine;
using System;
using System.Linq;

public abstract class ScriptableObjectWrapper<T> : ScriptableObject where T : ScriptableObjectWrapper<T>
{
    private static T[] cachedScriptableObjects = null;

    public static T[] GetAll(bool forceUpdate = false)
    {
        try
        {
            if (forceUpdate || cachedScriptableObjects == null)
            {
                cachedScriptableObjects = Resources.LoadAll<T>($"ScriptableObject/{typeof(T).Name}");
            }

            return cachedScriptableObjects;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return default;
        }
    }

    public static T Get(string name, bool forceUpdate = false)
    {
        return Get(x => x.name == name, forceUpdate);
    }

    public static T Get(Func<T, bool> predicate, bool forceUpdate = false)
    {
        try
        {
            return GetAll(forceUpdate).First(predicate);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return default;
        }
    }
}
