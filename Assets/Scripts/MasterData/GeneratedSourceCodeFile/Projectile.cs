using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterData
{
	public class Projectile : IMasterData
	{
		public int ID = new int();
		public string Name = "";
		public string ClassName = "";
		public string ModelResID = "";

		public void SetData(List<string> data)
		{
			Util.Convert(data[0], ref ID);
			Name = data[1];
			ClassName = data[2];
			ModelResID = data[3];
		}
	}
}

//  auto-generated file. do not modify
