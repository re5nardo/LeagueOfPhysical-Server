using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NetworkModel.Mirror;
using GameFramework;

public class EntityAnimatorController : LOPMonoEntityComponentBase
{
    // Note: not an object[] array because otherwise initialization is real annoying
    private int[] lastIntParameters;
    private float[] lastFloatParameters;
    private bool[] lastBoolParameters;
    private AnimatorControllerParameter[] parameters;

    // multiple layers
    private int[] animationHash;
    private int[] transitionHash;
    private float[] layerWeight;

    public override void OnAttached(IEntity entity)
    {
        base.OnAttached(entity);

        // store the animator parameters in a variable - the "Animator.parameters" getter allocates
        // a new parameter array every time it is accessed so we should avoid doing it in a loop
        parameters = Entity.ModelAnimator.parameters.Where(par => !Entity.ModelAnimator.IsParameterControlledByCurve(par.nameHash)).ToArray();
        lastIntParameters = new int[parameters.Length];
        lastFloatParameters = new float[parameters.Length];
        lastBoolParameters = new bool[parameters.Length];

        animationHash = new int[Entity.ModelAnimator.layerCount];
        transitionHash = new int[Entity.ModelAnimator.layerCount];
        layerWeight = new float[Entity.ModelAnimator.layerCount];

        SceneMessageBroker.AddSubscriber<EntityAnimatorSnap>(OnEntityAnimatorSnap).Where(snap => snap.entityId == Entity.EntityID);
    }

    public override void OnDetached()
    {
        base.OnDetached();

        SceneMessageBroker.RemoveSubscriber<EntityAnimatorSnap>(OnEntityAnimatorSnap);
    }

    private void OnEntityAnimatorSnap(EntityAnimatorSnap entityAnimatorSnap)
    {
        if (Entity.HasAuthority)
        {
            return;
        }

        var synchronization = ObjectPool.Instance.GetObject<SC_Synchronization>();
        synchronization.listSnap.Add(entityAnimatorSnap);

        RoomNetwork.Instance.SendToNear(synchronization, Entity.Position, LOP.Game.BROADCAST_SCOPE_RADIUS, instant: true);

        SyncAnimator(entityAnimatorSnap);
    }

    private void SyncAnimator(EntityAnimatorSnap entityAnimatorSnap)
    {
        Entity.ModelAnimator.speed = entityAnimatorSnap.animatorSpeed;

        for (int i = 0; i < parameters.Length; i++)
        {
            AnimatorControllerParameter par = parameters[i];
            if (par.type == AnimatorControllerParameterType.Int)
            {
                int newIntValue = (int)entityAnimatorSnap.animationParametersData.values[i];
                Entity.ModelAnimator.SetInteger(par.nameHash, newIntValue);
            }
            else if (par.type == AnimatorControllerParameterType.Float)
            {
                float newFloatValue = (float)entityAnimatorSnap.animationParametersData.values[i];
                Entity.ModelAnimator.SetFloat(par.nameHash, newFloatValue);
            }
            else if (par.type == AnimatorControllerParameterType.Bool)
            {
                bool newBoolValue = (bool)entityAnimatorSnap.animationParametersData.values[i];
                Entity.ModelAnimator.SetBool(par.nameHash, newBoolValue);
            }
        }

        entityAnimatorSnap.animStateDataList?.ForEach(animStateData =>
        {
            if (animStateData.stateHash != 0 && Entity.ModelAnimator.enabled)
            {
                Entity.ModelAnimator.Play(animStateData.stateHash, animStateData.layerId, animStateData.normalizedTime);
            }

            Entity.ModelAnimator.SetLayerWeight(animStateData.layerId, animStateData.weight);
        });
    }
}
