using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using Entity;
using System.Collections.Generic;
using GameFramework;

[TaskCategory("LOP")]
[TaskDescription("Check to see if the any object within the targets array is within sight")]
public class WithinSight : Conditional
{
    [Tooltip("The field of view angle (in degrees)")]
    public float fieldOfViewAngle;
    [Tooltip("How far out can the agent see")]
    public float viewMagnitude;
    [Tooltip("Returns success if this object becomes within sight")]
    public SharedTransform target;

	private LOPEntityBase entity = null;
	private LOPEntityBase Entity
	{
		get
		{
			if (entity == null)
			{
                entity = gameObject.GetComponent<LOPEntityBase>();
			}
			return entity;
		}
	}

	public override TaskStatus OnUpdate()
    {
		List<IEntity> listEntity = Entities.Get(Entity.Transform, fieldOfViewAngle, viewMagnitude, EntityRole.Player, new HashSet<int> { Entity.EntityID });
		listEntity.RemoveAll(x => !(x is Character));

		if (listEntity.Count > 0)
        {
			// set the target so other tasks will know which transform is within sight
			target.Value = (listEntity[0] as Character).Transform;	//	temp.. select just first item
			return TaskStatus.Success;
		}
       
        // a target is not within sight so return failure
        return TaskStatus.Failure;
    }

    // Draw the line of sight representation within the scene window
    public override void OnDrawGizmos()
    {
        //BehaviorDesigner.Samples.NPCViewUtilities.DrawLineOfSight(m_EntityBase.GetModelTransform(), fieldOfViewAngle, viewMagnitude);
    }
}
