﻿using UnityEngine;
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
                var scriptableObjects = Resources.FindObjectsOfTypeAll<T>();

                cachedScriptableObjects = scriptableObjects;
            }

            return cachedScriptableObjects;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
    }

    public static T Get(string name = "", bool forceUpdate = false)
    {
        try
        {
            return string.IsNullOrEmpty(name) ? GetAll(forceUpdate).First() : GetAll(forceUpdate).First(x => x.name == name);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }
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
            return null;
        }
    }
}
