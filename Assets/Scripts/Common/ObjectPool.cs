using System.Collections.Generic;
using GameFramework;

public class ObjectPool : Singleton<ObjectPool>
{
    private Dictionary<string, LinkedList<IPoolable>> m_dicObject = new Dictionary<string, LinkedList<IPoolable>>();
    private Dictionary<IPoolable, string> m_dicBeingUsed = new Dictionary<IPoolable, string>();

    public T GetObject<T>() where T : class, IPoolable, new()
    {
        T target = null;
        string typeName = typeof(T).Name;

        if (!m_dicObject.ContainsKey(typeName))
        {
            m_dicObject[typeName] = new LinkedList<IPoolable>();
        }

        if (m_dicObject[typeName].Count > 0)
        {
            target = m_dicObject[typeName].Last.Value as T;

            m_dicObject[typeName].RemoveLast();
        }
        else
        {
            target = new T();
        }

        m_dicBeingUsed[target] = typeName;

        return target;
    }

    public void ReturnObject(IPoolable obj)
    {
        obj.Clear();

        if (m_dicBeingUsed.ContainsKey(obj))
        {
            m_dicObject[m_dicBeingUsed[obj]].AddLast(obj);

            m_dicBeingUsed.Remove(obj);
        }
        else
        {
            UnityEngine.Debug.LogWarning(string.Format("Returned object is invalid! Type name : {0}", m_dicBeingUsed[obj]));
        }
    }
}
