using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

public class GlobalMonoBehavior : MonoSingleton<GlobalMonoBehavior>
{
    private bool isApplicationQuitting = false;
    public bool IsApplicationQuitting { get { return isApplicationQuitting; } }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void OnApplicationQuit()
    {
        isApplicationQuitting = true;
    }

    public new static Coroutine StartCoroutine(IEnumerator routine)
    {
        return (Instance as MonoBehaviour).StartCoroutine(routine);
    }

    public new static void StopCoroutine(IEnumerator routine)
    {
        (Instance as MonoBehaviour).StopCoroutine(routine);
    }

    public static void WaitUntil(Func<bool> predicate, Action action)
    {
        StartCoroutine(WaitUntilRoutine(predicate, action));
    }

    public static IEnumerator WaitUntilRoutine(Func<bool> predicate, Action action)
    {
        yield return new WaitUntil(predicate);

        action?.Invoke();
    }
}
