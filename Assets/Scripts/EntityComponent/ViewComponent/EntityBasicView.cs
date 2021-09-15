using System.Collections.Generic;
using UnityEngine;
using EntityMessage;
using GameFramework;
using UniRx;

public class EntityBasicView : MonoViewComponentBase
{
	private GameObject m_goModel = null;
    private Collider m_ColliderModel = null;
    private Animator m_AnimatorModel = null;
	private AnimationEventListener m_AnimationEventListener = null;
	private List<CollisionReporter> m_listModelCollisionReporter = new List<CollisionReporter>();
	private List<Renderer> m_listModelRenderer = new List<Renderer>();

    public Collider ModelCollider => m_ColliderModel;
    public Animator ModelAnimator => m_AnimatorModel;

    private Vector3 positionBeforePhysics;

    public override void OnAttached(IEntity entity)
    {
        base.OnAttached(entity);

        Entity.MessageBroker.Receive<ModelChanged>().Where(_ => IsValid).Subscribe(OnModelChanged);
        Entity.MessageBroker.Receive<AnimatorSetTrigger>().Where(_ => IsValid).Subscribe(OnAnimatorSetTrigger);
        Entity.MessageBroker.Receive<AnimatorSetFloat>().Where(_ => IsValid).Subscribe(OnAnimatorSetFloat);
        Entity.MessageBroker.Receive<AnimatorSetBool>().Where(_ => IsValid).Subscribe(OnAnimatorSetBool);
        Entity.MessageBroker.Receive<Destroying>().Where(_ => IsValid).Subscribe(OnDestroying);

        TickPubSubService.AddSubscriber("BeforePhysicsSimulation", OnBeforePhysicsSimulation);
        TickPubSubService.AddSubscriber("AfterPhysicsSimulation", OnAfterPhysicsSimulation);
    }

    public override void OnDetached()
    {
        base.OnDetached();

        TickPubSubService.RemoveSubscriber("BeforePhysicsSimulation", OnBeforePhysicsSimulation);
        TickPubSubService.RemoveSubscriber("AfterPhysicsSimulation", OnAfterPhysicsSimulation);
    }

    #region Command Handlers
    private void OnModelChanged(ModelChanged message)
    {
        ClearModel();
        SetModel(message.name);
    }

    private void OnAnimatorSetTrigger(AnimatorSetTrigger message)
    {
        Animator_SetTrigger(message.name);
    }

    private void OnAnimatorSetFloat(AnimatorSetFloat message)
    {
        Animator_SetFloat(message.name, message.value);
    }

    private void OnAnimatorSetBool(AnimatorSetBool message)
    {
        Animator_SetBool(message.name, message.value);
    }

    private void OnDestroying(Destroying message)
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
        m_goModel = model;
        m_goModel.transform.SetParent(Entity.Transform);
        m_goModel.transform.localPosition = Vector3.zero;
        m_goModel.transform.localRotation = Quaternion.identity;
        m_goModel.transform.localScale = Vector3.one;

        m_goModel.AddComponent<EntityIDTag>().SetEntityID(Entity.EntityID);

        m_ColliderModel = m_goModel.GetComponent<Collider>();
        m_AnimatorModel = m_goModel.GetComponent<Animator>();

        m_AnimationEventListener = m_goModel.GetComponent<AnimationEventListener>();
        if (m_AnimationEventListener != null)
        {
            m_AnimationEventListener.onAnimationEnd += OnAnimationEnd;
        }

        m_goModel.GetComponentsInChildren(true, m_listModelCollisionReporter);
        foreach (CollisionReporter reporter in m_listModelCollisionReporter)
        {
            reporter.onCollisionEnter += OnModelCollisionEnterHandler;
            reporter.onTriggerEnter += OnModelTriggerEnterHandler;
            reporter.onTriggerStay += OnModelTriggerStayHandler;
        }
        m_goModel.GetComponentsInChildren(true, m_listModelRenderer);
    }

	private void ClearModel()
	{
		if (m_goModel != null)
		{
			Destroy(m_goModel.GetComponent<EntityIDTag>());

			if (ResourcePool.HasInstance())
			{
				ResourcePool.Instance.ReturnResource(m_goModel);
			}
		}

		m_goModel = null;
        m_ColliderModel = null;
        m_AnimatorModel = null;

		if (m_AnimationEventListener != null)
		{
			m_AnimationEventListener.onAnimationEnd -= OnAnimationEnd;
			m_AnimationEventListener = null;
		}

		foreach (CollisionReporter reporter in m_listModelCollisionReporter)
		{
			reporter.onCollisionEnter -= OnModelCollisionEnterHandler;
			reporter.onTriggerEnter -= OnModelTriggerEnterHandler;
			reporter.onTriggerStay -= OnModelTriggerStayHandler;
		}

		m_listModelCollisionReporter.Clear();
		m_listModelRenderer.Clear();
	}

	private void OnAnimationEnd(string strAnimationName)
	{
		if (strAnimationName == "Die")
		{
            LOP.Game.Current.DestroyEntity(Entity.EntityID);
		}
	}

	#region Animator
	public void Animator_SetFloat(string name, float value)
	{
        if (m_AnimatorModel != null)
        {
            m_AnimatorModel.SetFloat(name, value);
        }
    }

	public void Animator_SetBool(string name, bool value)
	{
        if (m_AnimatorModel != null)
        {
            m_AnimatorModel.SetBool(name, value);
        }
    }

	public void Animator_SetTrigger(string name)
	{
        if (m_AnimatorModel != null)
        {
            m_AnimatorModel.SetTrigger(name);
        }
    }
	#endregion

	#region Model Collision Handler
	protected virtual void OnModelCollisionEnterHandler(Collider me, Collision collision)
	{
	}

	protected virtual void OnModelTriggerEnterHandler(Collider me, Collider other)
	{
	}

	protected virtual void OnModelTriggerStayHandler(Collider me, Collider other)
	{
	}
    #endregion

    #region PhysicsSimulation
    public virtual void OnBeforePhysicsSimulation(int tick)
    {
        positionBeforePhysics = Entity.Position;
    }
    
    public virtual void OnAfterPhysicsSimulation(int tick)
    {
        if (positionBeforePhysics != Entity.Position)
        {
            GamePubSubService.Publish(GameMessageKey.EntityMove, new object[] { Entity.EntityID });
        }
    }
    #endregion
}
