using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityIDTag : MonoBehaviour
{
	[SerializeField] private int m_nEntityID = -1;

	public void SetEntityID(int nEntityID)
    {
        m_nEntityID = nEntityID;
    }

    public int GetEntityID()
    {
        return m_nEntityID;
    }
}
