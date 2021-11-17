using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibleEffect : LOPMonoEntityComponentBase
{
    private float startTime;

    protected override void OnAttached(IEntity entity)
    {
        startTime = Game.Current.GameTime;
    }

    protected override void OnDetached()
    {
        Entity.ModelRenderers?.ForEach(renderer =>
        {
            renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1);
        });
    }

    private void LateUpdate()
    {
        float elapted = Game.Current.GameTime - startTime;

        float alpha = (elapted * 5) % 1;

        Entity.ModelRenderers?.ForEach(renderer =>
        {
            renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alpha);
        });
    }
}
