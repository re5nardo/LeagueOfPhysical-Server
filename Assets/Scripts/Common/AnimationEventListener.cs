using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventListener : MonoBehaviour
{
    public delegate void AnimationEndEventHandler(string strAnimationName);

    public event AnimationEndEventHandler onAnimationEnd = null;

    private void AnimationEnd(string strAnimationName)
    {
        onAnimationEnd?.Invoke(strAnimationName);
    }
}
