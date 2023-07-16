using System.Collections.Generic;
using UnityEngine;
using GameFramework;

public class EntityBasicView : LOPMonoEntityComponentBase
{
	private GameObject modelGameObject = null;
	private AnimationEventListener modelAnimationEventListener = null;

    public Renderer[] ModelRenderers { get; private set; }
    public Collider ModelCollider { get; private set; }
    public Animator ModelAnimator { get; private set; }

    protected override void OnAttached(IEntity entity)
    {
        Entity.MessageBroker.AddSubscriber<EntityMessage.ModelChanged>(OnModelChanged);
        Entity.MessageBroker.AddSubscriber<EntityMessage.AnimatorSetTrigger>(OnAnimatorSetTrigger);
        Entity.MessageBroker.AddSubscriber<EntityMessage.AnimatorSetFloat>(OnAnimatorSetFloat);
        Entity.MessageBroker.AddSubscriber<EntityMessage.AnimatorSetBool>(OnAnimatorSetBool);
        Entity.MessageBroker.AddSubscriber<EntityMessage.Destroying>(OnDestroying);

        SceneMessageBroker.AddSubscriber<TickMessage.BeforePhysicsSimulation>(OnBeforePhysicsSimulation);
        SceneMessageBroker.AddSubscriber<TickMessage.AfterPhysicsSimulation>(OnAfterPhysicsSimulation);
    }

    protected override void OnDetached()
    {
        Entity.MessageBroker.RemoveSubscriber<EntityMessage.ModelChanged>(OnModelChanged);
        Entity.MessageBroker.RemoveSubscriber<EntityMessage.AnimatorSetTrigger>(OnAnimatorSetTrigger);
        Entity.MessageBroker.RemoveSubscriber<EntityMessage.AnimatorSetFloat>(OnAnimatorSetFloat);
        Entity.MessageBroker.RemoveSubscriber<EntityMessage.AnimatorSetBool>(OnAnimatorSetBool);
        Entity.MessageBroker.RemoveSubscriber<EntityMessage.Destroying>(OnDestroying);

        SceneMessageBroker.RemoveSubscriber<TickMessage.BeforePhysicsSimulation>(OnBeforePhysicsSimulation);
        SceneMessageBroker.RemoveSubscriber<TickMessage.AfterPhysicsSimulation>(OnAfterPhysicsSimulation);
    }

    #region Command Handlers
    private void OnModelChanged(EntityMessage.ModelChanged message)
    {
        ClearModel();
        SetModel(message.name);
    }

    private void OnAnimatorSetTrigger(EntityMessage.AnimatorSetTrigger message)
    {
        Animator_SetTrigger(message.name);
    }

    private void OnAnimatorSetFloat(EntityMessage.AnimatorSetFloat message)
    {
        Animator_SetFloat(message.name, message.value);
    }

    private void OnAnimatorSetBool(EntityMessage.AnimatorSetBool message)
    {
        Animator_SetBool(message.name, message.value);
    }

    private void OnDestroying(EntityMessage.Destroying message)
    {
        Clear();
    }
    #endregion

	#region MonoBehaviour
	private void OnDestroy()
	{
        Clear();
	}
	#endregion

    private void Clear()
    {
        ClearModel();
    }

	private void SetModel(string strModel)
	{
        SetModel(ResourcePool.Instance.GetResource(strModel));
    }

    public void SetModel(GameObject model)
    {
        modelGameObject = model;
        modelGameObject.transform.SetParent(Entity.Transform);
        modelGameObject.transform.localPosition = Vector3.zero;
        modelGameObject.transform.localRotation = Quaternion.identity;
        modelGameObject.transform.localScale = Vector3.one;

        modelGameObject.AddComponent<EntityIDTag>().SetEntityID(Entity.EntityId);

        ModelCollider = modelGameObject.GetComponent<Collider>();
        ModelAnimator = modelGameObject.GetComponent<Animator>();

        modelAnimationEventListener = modelGameObject.GetComponent<AnimationEventListener>();
        if (modelAnimationEventListener != null)
        {
            modelAnimationEventListener.onAnimationEnd += OnAnimationEnd;
        }

        Entity.CollisionReporter.onCollisionEnter += OnModelCollisionEnterHandler;
        Entity.CollisionReporter.onTriggerEnter += OnModelTriggerEnterHandler;
        Entity.CollisionReporter.onTriggerStay += OnModelTriggerStayHandler;

        ModelRenderers = modelGameObject.GetComponentsInChildren<Renderer>(true);
    }

	private void ClearModel()
	{
		if (modelGameObject != null)
		{
			Destroy(modelGameObject.GetComponent<EntityIDTag>());

			if (ResourcePool.HasInstance())
			{
				ResourcePool.Instance.ReturnResource(modelGameObject);
			}
		}

        modelGameObject = null;
        ModelCollider = null;
        ModelAnimator = null;

		if (modelAnimationEventListener != null)
		{
            modelAnimationEventListener.onAnimationEnd -= OnAnimationEnd;
            modelAnimationEventListener = null;
		}

        Entity.CollisionReporter.onCollisionEnter -= OnModelCollisionEnterHandler;
        Entity.CollisionReporter.onTriggerEnter -= OnModelTriggerEnterHandler;
        Entity.CollisionReporter.onTriggerStay -= OnModelTriggerStayHandler;

        ModelRenderers = null;
    }

	private void OnAnimationEnd(string strAnimationName)
	{
		if (strAnimationName == "Die")
		{
            LOP.Game.Current.DestroyEntity(Entity.EntityId);
		}
	}

	#region Animator
	public void Animator_SetFloat(string name, float value)
	{
        if (ModelAnimator != null)
        {
            ModelAnimator.SetFloat(name, value);
        }
    }

	public void Animator_SetBool(string name, bool value)
	{
        if (ModelAnimator != null)
        {
            ModelAnimator.SetBool(name, value);
        }
    }

	public void Animator_SetTrigger(string name)
	{
        if (ModelAnimator != null)
        {
            ModelAnimator.SetTrigger(name);
        }
    }
	#endregion

	#region Model Collision Handler
	protected virtual void OnModelCollisionEnterHandler(Collider me, Collision collision)
	{
	}

	protected virtual void OnModelTriggerEnterHandler(Collider me, Collider other)
	{
        SceneMessageBroker.Publish(new EntityMessage.ModelTriggerEnter(Entity.EntityId, me, other));
    }

	protected virtual void OnModelTriggerStayHandler(Collider me, Collider other)
	{
	}
    #endregion

    #region Physics Simulation
    protected virtual void OnBeforePhysicsSimulation(TickMessage.BeforePhysicsSimulation message)
    {
        Entity.Blackboard.Set("positionBeforePhysics", Entity.Position);
    }

    protected virtual void OnAfterPhysicsSimulation(TickMessage.AfterPhysicsSimulation message)
    {
        if (Entity.Blackboard.Get<Vector3>("positionBeforePhysics", true) != Entity.Position)
        {
            SceneMessageBroker.Publish(new GameMessage.EntityMove(Entity.EntityId));
        }
    }
    #endregion
}
