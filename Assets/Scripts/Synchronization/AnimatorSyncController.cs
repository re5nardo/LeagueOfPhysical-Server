using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System.Linq;

public class AnimatorSyncController : LOPMonoEntitySyncControllerBase<AnimatorSyncData>
{
    private AnimatorSyncData animatorSyncData = new AnimatorSyncData();
    private AnimatorSyncData lastSyncData = new AnimatorSyncData();

    // Note: not an object[] array because otherwise initialization is real annoying
    private int[] lastIntParameters;
    private float[] lastFloatParameters;
    private bool[] lastBoolParameters;
    private AnimatorControllerParameter[] parameters;

    // multiple layers
    private int[] animationHash;
    private int[] transitionHash;
    private float[] layerWeight;

    public override void OnInitialize()
    {
        base.OnInitialize();

        // store the animator parameters in a variable - the "Animator.parameters" getter allocates
        // a new parameter array every time it is accessed so we should avoid doing it in a loop
        parameters = Entity.ModelAnimator.parameters.Where(par => !Entity.ModelAnimator.IsParameterControlledByCurve(par.nameHash)).ToArray();
        lastIntParameters = new int[parameters.Length];
        lastFloatParameters = new float[parameters.Length];
        lastBoolParameters = new bool[parameters.Length];

        animationHash = new int[Entity.ModelAnimator.layerCount];
        transitionHash = new int[Entity.ModelAnimator.layerCount];
        layerWeight = new float[Entity.ModelAnimator.layerCount];

        SceneMessageBroker.AddSubscriber<TickMessage.LateTickEnd>(OnLateTickEnd);
    }

    public override void OnFinalize()
    {
        base.OnFinalize();

        SceneMessageBroker.RemoveSubscriber<TickMessage.LateTickEnd>(OnLateTickEnd);
    }

    private void OnLateTickEnd(TickMessage.LateTickEnd message)
    {
        if (HasAuthority)
        {
            if (lastSyncData == null || lastSyncData.ObjectToHash().SequenceEqual(GetSyncData().ObjectToHash()) == false)
            {
                var syncData = GetSyncData();
                Sync(syncData);

                lastSyncData = syncData;
            }
        }
    }

    public override AnimatorSyncData GetSyncData()
    {
        return SetAnimatorSyncData(animatorSyncData);
    }

    public override void OnSync(AnimatorSyncData value)
    {
        if (HasAuthority)
        {
            return;
        }

        SyncAnimator(value);

        lastSyncData = value;
    }

    private void SyncAnimator(AnimatorSyncData animatorSyncData)
    {
        Entity.ModelAnimator.speed = animatorSyncData.animatorSpeed;

        for (int i = 0; i < parameters.Length; i++)
        {
            AnimatorControllerParameter par = parameters[i];
            if (par.type == AnimatorControllerParameterType.Int)
            {
                int newIntValue = (int)animatorSyncData.animationParametersData.values[i];
                Entity.ModelAnimator.SetInteger(par.nameHash, newIntValue);
            }
            else if (par.type == AnimatorControllerParameterType.Float)
            {
                float newFloatValue = (float)animatorSyncData.animationParametersData.values[i];
                Entity.ModelAnimator.SetFloat(par.nameHash, newFloatValue);
            }
            else if (par.type == AnimatorControllerParameterType.Bool)
            {
                bool newBoolValue = (bool)animatorSyncData.animationParametersData.values[i];
                Entity.ModelAnimator.SetBool(par.nameHash, newBoolValue);
            }
        }

        animatorSyncData.animStateDataList?.ForEach(animStateData =>
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

    private AnimatorSyncData SetAnimatorSyncData(AnimatorSyncData animatorSyncData)
    {
        animatorSyncData.animatorSpeed = Entity.ModelAnimator.speed;

        animatorSyncData.animationParametersData.values.Clear();
        for (int i = 0; i < Entity.ModelAnimator.parameters.Length; i++)
        {
            AnimatorControllerParameter par = Entity.ModelAnimator.parameters[i];
            if (par.type == AnimatorControllerParameterType.Int)
            {
                int value = Entity.ModelAnimator.GetInteger(par.nameHash);
                animatorSyncData.animationParametersData.values.Add(value);
            }
            else if (par.type == AnimatorControllerParameterType.Float)
            {
                float value = Entity.ModelAnimator.GetFloat(par.nameHash);
                animatorSyncData.animationParametersData.values.Add(value);
            }
            else if (par.type == AnimatorControllerParameterType.Bool)
            {
                bool value = Entity.ModelAnimator.GetBool(par.nameHash);
                animatorSyncData.animationParametersData.values.Add(value);
            }
        }

        animatorSyncData.animStateDataList.Clear();
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

            animatorSyncData.animStateDataList.Add(animStateData);
        }

        return animatorSyncData;
    }
}
