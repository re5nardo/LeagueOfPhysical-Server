using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Entity;
using NetworkModel.Mirror;
using GameFramework;

public class EntityAnimatorController : MonoBehaviour
{
    private LOPEntityBase entity;
    private RoomProtocolDispatcher roomProtocolDispatcher;

    // Note: not an object[] array because otherwise initialization is real annoying
    private int[] lastIntParameters;
    private float[] lastFloatParameters;
    private bool[] lastBoolParameters;
    private AnimatorControllerParameter[] parameters;

    // multiple layers
    private int[] animationHash;
    private int[] transitionHash;
    private float[] layerWeight;
 
    private void Awake()
    {
        entity = GetComponent<LOPEntityBase>();

        roomProtocolDispatcher = gameObject.AddComponent<RoomProtocolDispatcher>();
        roomProtocolDispatcher[typeof(CS_Synchronization)] = OnCS_Synchronization;

        // store the animator parameters in a variable - the "Animator.parameters" getter allocates
        // a new parameter array every time it is accessed so we should avoid doing it in a loop
        parameters = entity.ModelAnimator.parameters.Where(par => !entity.ModelAnimator.IsParameterControlledByCurve(par.nameHash)).ToArray();
        lastIntParameters = new int[parameters.Length];
        lastFloatParameters = new float[parameters.Length];
        lastBoolParameters = new bool[parameters.Length];

        animationHash = new int[entity.ModelAnimator.layerCount];
        transitionHash = new int[entity.ModelAnimator.layerCount];
        layerWeight = new float[entity.ModelAnimator.layerCount];
    }

    private void OnCS_Synchronization(IMessage msg)
    {
        if (entity.HasAuthority)
        {
            return;
        }

        CS_Synchronization synchronization = msg as CS_Synchronization;

        synchronization.listSnap?.ForEach(snap =>
        {
            if (snap is EntityAnimatorSnap entityAnimatorSnap && entityAnimatorSnap.entityId == entity.EntityID)
            {
                var synchronization = ObjectPool.Instance.GetObject<SC_Synchronization>();
                synchronization.listSnap.Add(entityAnimatorSnap);

                RoomNetwork.Instance.SendToAll(synchronization, instant: true);

                SyncAnimator(entityAnimatorSnap);
            }
        });
    }

    private void SyncAnimator(EntityAnimatorSnap entityAnimatorSnap)
    {
        entity.ModelAnimator.speed = entityAnimatorSnap.animatorSpeed;

        for (int i = 0; i < parameters.Length; i++)
        {
            AnimatorControllerParameter par = parameters[i];
            if (par.type == AnimatorControllerParameterType.Int)
            {
                int newIntValue = (int)entityAnimatorSnap.animationParametersData.values[i];
                entity.ModelAnimator.SetInteger(par.nameHash, newIntValue);
            }
            else if (par.type == AnimatorControllerParameterType.Float)
            {
                float newFloatValue = (float)entityAnimatorSnap.animationParametersData.values[i];
                entity.ModelAnimator.SetFloat(par.nameHash, newFloatValue);
            }
            else if (par.type == AnimatorControllerParameterType.Bool)
            {
                bool newBoolValue = (bool)entityAnimatorSnap.animationParametersData.values[i];
                entity.ModelAnimator.SetBool(par.nameHash, newBoolValue);
            }
        }

        entityAnimatorSnap.animStateDataList?.ForEach(animStateData =>
        {
            if (animStateData.stateHash != 0 && entity.ModelAnimator.enabled)
            {
                entity.ModelAnimator.Play(animStateData.stateHash, animStateData.layerId, animStateData.normalizedTime);
            }

            entity.ModelAnimator.SetLayerWeight(animStateData.layerId, animStateData.weight);
        });
    }
}
