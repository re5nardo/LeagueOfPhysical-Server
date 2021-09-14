using System.Collections;
using UnityEngine;
using Entity;

public abstract class ControllerBase : MonoBehaviour
{
    protected LOPMonoEntityBase Entity = null;

    public void Possess(LOPMonoEntityBase entity)
    {
		Entity = entity;

        OnPossessed();

        StartCoroutine("UpdateLoop");
    }

    public void UnPossess()
    {
		Entity = null;

        OnUnPossessed();

        StopCoroutine("UpdateLoop");
    }

    public bool IsPossessed()
    {
        return Entity != null;
    }

    private IEnumerator UpdateLoop()
    {
        while (true)
        {
            UpdateBody();

            yield return null;
        }
    }

    protected virtual void OnPossessed()
    {
    }

    protected virtual void OnUnPossessed()
    {
    }

    protected abstract void UpdateBody();
}
