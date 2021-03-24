using UnityEngine;
using System.Collections.Generic;
using System;

public static class Extension
{
    public static Vector3 ToVector3(this string str)
    {
        if (str.StartsWith("(") && str.EndsWith(")"))
        {
            str = str.Substring(1, str.Length - 2);
        }

        string[] arrStr = str.Split(',');

        return new Vector3(float.Parse(arrStr[0]), float.Parse(arrStr[1]), float.Parse(arrStr[2]));
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        var component = gameObject.GetComponent<T>();
        if (component == null)
        {

            component = gameObject.AddComponent<T>();
        }

        return component;
    }

    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (T item in enumerable)
        {
            action.Invoke(item);
        }
    }
}
