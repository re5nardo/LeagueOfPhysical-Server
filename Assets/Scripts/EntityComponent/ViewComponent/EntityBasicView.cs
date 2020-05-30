using System.Collections.Generic;
using UnityEngine;
using EntityCommand;
using GameFramework;

public class EntityBasicView : MonoViewComponentBase
{
	private GameObject m_goModel = null;
	private Transform m_trModel = null;
	private Animator m_AnimatorModel = null;
	private AnimationEventListener m_AnimationEventListener = null;
	private List<CollisionReporter> m_listModelCollisionReporter = new List<CollisionReporter>();
	private List<Renderer> m_listModelRenderer = new List<Renderer>();


    public override void OnAttached(IEntity entity)
    {
        base.OnAttached(entity);

        CommandHandlerOn(typeof(ModelChanged), OnModelChanged);
        CommandHandlerOn(typeof(PositionChanged), OnPositionChanged);
        CommandHandlerOn(typeof(RotationChanged), OnRotationChanged);
        CommandHandlerOn(typeof(AnimatorSetTrigger), OnAnimatorSetTrigger);
        CommandHandlerOn(typeof(AnimatorSetFloat), OnAnimatorSetFloat);
        CommandHandlerOn(typeof(AnimatorSetBool), OnAnimatorSetBool);
        CommandHandlerOn(typeof(Destroying), OnDestroying);
    }

    public override void OnDetached()
    {
        base.OnDetached();

        CommandHandlerOff(typeof(ModelChanged));
        CommandHandlerOff(typeof(PositionChanged));
        CommandHandlerOff(typeof(RotationChanged));
        CommandHandlerOff(typeof(AnimatorSetTrigger));
        CommandHandlerOff(typeof(AnimatorSetFloat));
        CommandHandlerOff(typeof(AnimatorSetBool));
        CommandHandlerOff(typeof(Destroying));
    }

    #region Command Handlers
    private void OnModelChanged(ICommand command)
    {
        ModelChanged cmd = command as ModelChanged;

        ClearModel();
        SetModel(cmd.name);
    }

    private void OnPositionChanged(ICommand command)
    {
        Position = Entity.Position;
    }

    private void OnRotationChanged(ICommand command)
    {
        Rotation = Entity.Rotation;
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
		m_goModel = ResourcePool.Instance.GetResource(strModel);
		m_goModel.transform.parent = null;
		m_trModel = m_goModel.transform;

		m_goModel.AddComponent<EntityIDTag>().SetEntityID(Entity.EntityID);

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

			if (ResourcePool.IsInstantiated())
			{
				ResourcePool.Instance.ReturnResource(m_goModel);
			}
		}

		m_goModel = null;
		m_trModel = null;
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

	public Transform ModelTransform { get { return m_trModel; } }

	public Vector3 Position
	{
		get { return m_trModel.position; }
		set { m_trModel.position = value; }
	}

	public Vector3 Rotation
	{
		get { return m_trModel.rotation.eulerAngles; }
		set { m_trModel.rotation = Quaternion.Euler(value); }
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
		m_AnimatorModel?.SetFloat(name, value);
	}

	public void Animator_SetBool(string name, bool value)
	{
		m_AnimatorModel?.SetBool(name, value);
	}

	public void Animator_SetTrigger(string name)
	{
		m_AnimatorModel?.SetTrigger(name);
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
}
