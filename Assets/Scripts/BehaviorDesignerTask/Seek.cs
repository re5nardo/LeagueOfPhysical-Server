using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using Entity;

// Move towards the target specified
[TaskCategory("LOP")]
public class Seek : Action
{
    [Tooltip("The target that the agent is seeking")]
    public SharedTransform target;

	private MonoEntityBase Entity__ = null;
	private MonoEntityBase Entity
	{
		get
		{
			if (Entity__ == null)
			{
				Entity__ = gameObject.GetComponent<MonoEntityBase>();
			}
			return Entity__;
		}
	}

	public override TaskStatus OnUpdate()
    {
        Vector3 vec3Direction = Entity.Position - target.Value.position;

        if (vec3Direction.sqrMagnitude > 100)
        {
            return TaskStatus.Failure;
        }
        else if (vec3Direction.sqrMagnitude < 5)
        {
            return TaskStatus.Success;
        }

		BehaviorController behaviorController = Entity.GetComponent<BehaviorController>();
        behaviorController?.Move(target.Value.position);

		return TaskStatus.Running;
    }
}
