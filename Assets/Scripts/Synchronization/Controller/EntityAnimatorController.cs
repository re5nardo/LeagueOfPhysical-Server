using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NetworkModel.Mirror;
using GameFramework;

public class EntityAnimatorController : LOPMonoEntityComponentBase
{
    private EntityAnimatorSnap entityAnimatorSnap = new EntityAnimatorSnap();

    // Note: not an object[] array because otherwise initialization is real annoying
    private int[] lastIntParameters;
    private float[] lastFloatParameters;
    private bool[] lastBoolParameters;
    private AnimatorControllerParameter[] parameters;

    // multiple layers
    private int[] animationHash;
    private int[] transitionHash;
    private float[] layerWeight;

    protected override void OnAttached(IEntity entity)
    {
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
        SceneMessageBroker.AddSubscriber<TickMessage.LateTickEnd>(OnLateTickEnd);
    }

    protected override void OnDetached()
    {
        SceneMessageBroker.RemoveSubscriber<EntityAnimatorSnap>(OnEntityAnimatorSnap);
        SceneMessageBroker.RemoveSubscriber<TickMessage.LateTickEnd>(OnLateTickEnd);
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

    private void OnLateTickEnd(TickMessage.LateTickEnd message)
    {
        if (Entity.HasAuthority)
        {
            var synchronization = ObjectPool.Instance.GetObject<SC_Synchronization>();
            SetEntityAnimatorSnap(entityAnimatorSnap);
            synchronization.listSnap.Add(entityAnimatorSnap);

            RoomNetwork.Instance.SendToNear(synchronization, Entity.Position, LOP.Game.BROADCAST_SCOPE_RADIUS, instant: true);
        }
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

    private bool CheckAnimStateChanged(out int stateHash, out float normalizedTime, int layerId)
    {
        bool change = false;
        stateHash = 0;
        normalizedTime = 0;

        float lw = Entity.ModelAnimator.GetLayerWeight(layerId);
        if (Mathf.Abs(lw - layerWeight[layerId]) > 0.001f)
        {
            layerWeight[layerId] = lw;
            change = true;
        }

        if (Entity.ModelAnimator.IsInTransition(layerId))
        {
            AnimatorTransitionInfo tt = Entity.ModelAnimator.GetAnimatorTransitionInfo(layerId);
            if (tt.fullPathHash != transitionHash[layerId])
            {
                // first time in this transition
                transitionHash[layerId] = tt.fullPathHash;
                animationHash[layerId] = 0;
                return true;
            }
            return change;
        }

        AnimatorStateInfo st = Entity.ModelAnimator.GetCurrentAnimatorStateInfo(layerId);
        if (st.fullPathHash != animationHash[layerId])
        {
            // first time in this animation state
            if (animationHash[layerId] != 0)
            {
                // came from another animation directly - from Play()
                stateHash = st.fullPathHash;
                normalizedTime = st.normalizedTime;
            }
            transitionHash[layerId] = 0;
            animationHash[layerId] = st.fullPathHash;
            return true;
        }

        return change;
    }

    private void SetEntityAnimatorSnap(EntityAnimatorSnap entityAnimatorSnap)
    {
        entityAnimatorSnap.Tick = Game.Current.CurrentTick;
        entityAnimatorSnap.entityId = Entity.EntityID;
        entityAnimatorSnap.animatorSpeed = Entity.ModelAnimator.speed;

        entityAnimatorSnap.animationParametersData.values.Clear();
        for (int i = 0; i < Entity.ModelAnimator.parameters.Length; i++)
        {
            AnimatorControllerParameter par = Entity.ModelAnimator.parameters[i];
            if (par.type == AnimatorControllerParameterType.Int)
            {
                int value = Entity.ModelAnimator.GetInteger(par.nameHash);
                entityAnimatorSnap.animationParametersData.values.Add(value);
            }
            else if (par.type == AnimatorControllerParameterType.Float)
            {
                float value = Entity.ModelAnimator.GetFloat(par.nameHash);
                entityAnimatorSnap.animationParametersData.values.Add(value);
            }
            else if (par.type == AnimatorControllerParameterType.Bool)
            {
                bool value = Entity.ModelAnimator.GetBool(par.nameHash);
                entityAnimatorSnap.animationParametersData.values.Add(value);
            }
        }

        entityAnimatorSnap.animStateDataList.Clear();
        for (int i = 0; i < Entity.ModelAnimator.layerCount; i++)
        {
            if (!CheckAnimStateChanged(out var stateHash, out var normalizedTime, i))
            {
                continue;
            }

            AnimStateData animStateData = new AnimStateData();
            animStateData.stateHash = stateHash;
            animStateData.normalizedTime = normalizedTime;
            animStateData.layerId = i;
            animStateData.weight = layerWeight[i];

            entityAnimatorSnap.animStateDataList.Add(animStateData);
        }
    }
}
