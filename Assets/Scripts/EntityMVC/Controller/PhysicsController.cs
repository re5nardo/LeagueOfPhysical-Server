using System.Collections.Generic;
using System;
using EntityCommand;
using GameFramework;

public class PhysicsController : MonoControllerComponentBase
{
	public override void OnCommand(ICommand command)
	{
		base.OnCommand(command);

		if (command is ModelTriggerEnter)
		{
			Entity.SendCommand(command, new List<Type> { typeof(IModelComponent) });
		}
	}
}
