using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class ResourcePool : MonoSingleton<ResourcePool>
{
    private Dictionary<string, LinkedList<GameObject>> m_dicResource = new Dictionary<string, LinkedList<GameObject>>();
    private Dictionary<GameObject, string> m_dicBeingUsed = new Dictionary<GameObject, string>();

    public GameObject GetResource(string strPath, Transform trParent = null)
    {
        GameObject target = null;

        if (!m_dicResource.ContainsKey(strPath))
        {
            m_dicResource[strPath] = new LinkedList<GameObject>();
        }

        if (m_dicResource[strPath].Count > 0)
        {
            target = m_dicResource[strPath].Last.Value;

            m_dicResource[strPath].RemoveLast();
        }
        else
        {
            target = Instantiate(Resources.Load(strPath)) as GameObject;
        }

        m_dicBeingUsed[target] = strPath;   //  IResourcePoolable - Clear, SetPath, GetPath..?

        target.transform.SetParent(trParent);
        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;
        target.transform.localScale = Vector3.one;

        target.SetActive(true);

        return target;
    }

    public void ReturnResource(GameObject go)
    {
        if (m_dicBeingUsed.ContainsKey(go))
        {
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            go.SetActive(false);

            m_dicResource[m_dicBeingUsed[go]].AddLast(go);

            m_dicBeingUsed.Remove(go);
        }
        else
        {
            Debug.LogWarning(string.Format("Returned resource is invalid! go.name : {0}", go.name));
        }
    }
}
