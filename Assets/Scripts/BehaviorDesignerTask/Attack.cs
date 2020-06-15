using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using Entity;

[TaskCategory("LOP")]
public class Attack : Action
{
    [Tooltip("The target that the agent is attacking")]
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
		int characterID = (Entity as Character).MasterData.ID;                          //  Temp
		int attackID = 7 + (characterID - Define.MasterData.CharacterID.EVELYNN) * 4;	//  Temp
        var behaviors = Entity.GetComponents<Behavior.BehaviorBase>();
        foreach(var behavior in behaviors)
        {
            if(behavior.GetBehaviorMasterID() == attackID && behavior.IsPlaying())
            {
                return TaskStatus.Running;
            }
        }

        Vector3 vec3Direction = Entity.Position - target.Value.position;
		BehaviorController behaviorController = Entity.GetComponent<BehaviorController>();

		if (vec3Direction.sqrMagnitude > 100)
        {
            return TaskStatus.Failure;
        }
        else if (vec3Direction.sqrMagnitude < 5)
        {
            behaviorController?.StartBehavior(attackID);
			return TaskStatus.Running;
        }

        behaviorController?.Move(target.Value.position);
		return TaskStatus.Running;
    }
}
