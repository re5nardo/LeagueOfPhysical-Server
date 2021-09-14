using System.Collections.Generic;
using UnityEngine;
using EntityCommand;
using GameFramework;
using Entity;

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

    protected LOPMonoEntityBase lopEntity;

    public override void OnAttached(IEntity entity)
    {
        base.OnAttached(entity);

        lopEntity = entity as LOPMonoEntityBase;

        AddCommandHandler(typeof(ModelChanged), OnModelChanged);
        AddCommandHandler(typeof(AnimatorSetTrigger), OnAnimatorSetTrigger);
        AddCommandHandler(typeof(AnimatorSetFloat), OnAnimatorSetFloat);
        AddCommandHandler(typeof(AnimatorSetBool), OnAnimatorSetBool);
        AddCommandHandler(typeof(Destroying), OnDestroying);

        TickPubSubService.AddSubscriber("BeforePhysicsSimulation", OnBeforePhysicsSimulation);
        TickPubSubService.AddSubscriber("AfterPhysicsSimulation", OnAfterPhysicsSimulation);
    }

    public override void OnDetached()
    {
        base.OnDetached();

        lopEntity = null;

        RemoveCommandHandler(typeof(ModelChanged), OnModelChanged);
        RemoveCommandHandler(typeof(AnimatorSetTrigger), OnAnimatorSetTrigger);
        RemoveCommandHandler(typeof(AnimatorSetFloat), OnAnimatorSetFloat);
        RemoveCommandHandler(typeof(AnimatorSetBool), OnAnimatorSetBool);
        RemoveCommandHandler(typeof(Destroying), OnDestroying);

        TickPubSubService.RemoveSubscriber("BeforePhysicsSimulation", OnBeforePhysicsSimulation);
        TickPubSubService.RemoveSubscriber("AfterPhysicsSimulation", OnAfterPhysicsSimulation);
    }

    #region Command Handlers
    private void OnModelChanged(ICommand command)
    {
        ModelChanged cmd = command as ModelChanged;

        ClearModel();
        SetModel(cmd.name);
    }

    private void OnAnimatorSetTrigger(ICommand command)
    {
        Animator_SetTrigger((command as AnimatorSetTrigger).name);
    }

    private void OnAnimatorSetFloat(ICommand command)
    {
        AnimatorSetFloat cmd = command as AnimatorSetFloat;

        Animator_SetFloat(cmd.name, cmd.value);
    }

    private void OnAnimatorSetBool(ICommand command)
    {
        AnimatorSetBool cmd = command as AnimatorSetBool;

        Animator_SetBool(cmd.name, cmd.value);
    }

    private void OnDestroying(ICommand command)
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
        m_goModel.transform.SetParent(lopEntity.Transform);
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
