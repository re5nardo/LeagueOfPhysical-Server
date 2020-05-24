using UnityEngine;
using EntityCommand;
using System.Collections.Generic;
using System;
using GameFramework;

public class BasicModel : MonoModelComponentBase
{
	protected string m_strModel = "";

	public override void Initialize(params object[] param)
	{
		base.Initialize(param);

		m_strModel = (string)param[0];
		ModelChanged command = new ModelChanged(m_strModel);
		Entity.SendCommand(command, new List<Type> { typeof(IViewComponent) });
	}

	public string ModelName { get { return m_strModel; } }
}
