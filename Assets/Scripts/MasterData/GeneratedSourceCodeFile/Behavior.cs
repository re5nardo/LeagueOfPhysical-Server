using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterData
{
	public class Behavior : IMasterData
	{
		public int ID = new int();
		public string Name = "";
		public string ClassName = "";
		public List<string> ClassParams = new List<string>();
		public float Lifespan = new float();
		public List<int> CompatibleBehaviorIDs = new List<int>();

		public void SetData(List<string> data)
		{
			Util.Convert(data[0], ref ID);
			Name = data[1];
			ClassName = data[2];
			Util.Parse(data[3], ',', ClassParams);
			Util.Convert(data[4], ref Lifespan);
			Util.Parse(data[5], ',', CompatibleBehaviorIDs);
		}
	}
}

//  auto-generated file. do not modify
