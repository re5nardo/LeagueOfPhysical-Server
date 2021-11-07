using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard : LOPMonoEntityComponentBase
{
    private Dictionary<string, object> objects = new Dictionary<string, object>();

    public T Set<T>(string key, T value)
    {
        objects[key] = value;

        return value;
    }

    public T Get<T>(string key, bool remove = false)
    {
        var value = (T)objects[key];

        if (remove)
        {
            Remove(key);
        }

        return value;
    }

    public void Remove(string key)
    {
        if (objects.ContainsKey(key))
        {
            objects.Remove(key);
        }
    }
}
