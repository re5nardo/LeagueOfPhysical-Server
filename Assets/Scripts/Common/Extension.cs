using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;

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

    public static StringBuilder AppendTab(this StringBuilder stringBuilder)
    {
        return stringBuilder.Append("\t");
    }
}
