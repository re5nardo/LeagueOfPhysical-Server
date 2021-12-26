using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using GameFramework;

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

    public static byte[] ObjectToHash(this object obj)
    {
        if (obj == null) throw new Exception("obj is null!");

        using (var memoryStream = new MemoryStream())
        {
            Util.Formatter.Serialize(memoryStream, obj);

            using (var sha1 = SHA1.Create())
            {
                return sha1.ComputeHash(memoryStream.ToArray());
            }
        }
    }

    public static float GameTime(this SyncDataEntry value)
    {
        return value.meta.tick * Game.Current.TickInterval;
    }
}
