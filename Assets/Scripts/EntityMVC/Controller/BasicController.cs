using UnityEngine;
using Behavior;
using State;
using GameFramework;

public class BasicController : MonoControllerComponentBase
{
	#region Behavior
	public void Move(Vector3 vec3Destination)
	{
		Vector3 vec3Direction = vec3Destination - Entity.Position;

		Move oldMove = Entity.GetComponent<Move>();
		if (oldMove != null)
		{
			oldMove.SetDestination(vec3Destination);
		}
		else
		{
			Move move = BehaviorFactory.Instance.CreateBehavior(gameObject, MasterDataDefine.BehaviorID.MOVE) as Move;
			Entity.AttachComponent(move);
			move.SetData(MasterDataDefine.BehaviorID.MOVE, vec3Destination);
			move.onBehaviorEnd += BehaviorHelper.BehaviorDestroyer;

			move.StartBehavior();
		}

		Rotation oldRotation = Entity.GetComponent<Rotation>();
		if (oldRotation != null)
		{
			oldRotation.SetDirection(vec3Direction);
		}
		else
		{
			Rotation rotation = BehaviorFactory.Instance.CreateBehavior(gameObject, MasterDataDefine.BehaviorID.ROTATION) as Rotation;
			Entity.AttachComponent(rotation);
			rotation.SetData(MasterDataDefine.BehaviorID.ROTATION, vec3Direction);
			rotation.onBehaviorEnd += BehaviorHelper.BehaviorDestroyer;

			rotation.StartBehavior();
		}
	}

	public void Die()
	{
		var behaviors = Entity.GetComponents<BehaviorBase>();
		foreach (var behavior in behaviors)
		{
			if (behavior.IsPlaying())
				behavior.StopBehavior();
		}

		var states = Entity.GetComponents<StateBase>();
		foreach (var state in states)
		{
			if (state.IsPlaying())
				state.StopState();
		}
	}

	public void StartBehavior(int nBehaviorMasterID, params object[] param)
	{
		BehaviorBase behavior = BehaviorFactory.Instance.CreateBehavior(gameObject, nBehaviorMasterID);
		Entity.AttachComponent(behavior);
		behavior.SetData(nBehaviorMasterID, param);
		behavior.onBehaviorEnd += BehaviorHelper.BehaviorDestroyer;

		behavior.StartBehavior();
	}
	#endregion

	#region State
	public void StartState(int nStateMasterID, params object[] param)
	{
		StateBase state = StateFactory.Instance.CreateState(gameObject, nStateMasterID);
		Entity.AttachComponent(state);
		state.SetData(nStateMasterID, param);
		state.onStateEnd += StateHelper.StateDestroyer;

		state.StartState();
	}
	#endregion
}
