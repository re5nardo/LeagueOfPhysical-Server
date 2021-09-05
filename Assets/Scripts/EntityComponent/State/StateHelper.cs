using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using State;

public class StateHelper
{
	public static void StateDestroyer(StateBase state)
	{
		state.Entity.DetachEntityComponent(state);

		Object.Destroy(state);
	}
}
