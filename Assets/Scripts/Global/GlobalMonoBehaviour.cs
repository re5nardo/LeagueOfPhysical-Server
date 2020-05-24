using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMonoBehavior : MonoBehaviour
{
    private bool isApplicationQuitting = false;
    public bool IsApplicationQuitting { get { return isApplicationQuitting; } }

    private void OnApplicationQuit()
    {
        isApplicationQuitting = true;
    }
}
