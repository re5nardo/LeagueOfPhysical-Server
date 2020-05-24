using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterData
{
	public class GameItem : IMasterData
	{
		public int ID = new int();
		public string Name = "";
		public string ClassName = "";
		public int Price = new int();
		public float Lifespan = new float();
		public List<string> Properties = new List<string>();
		public int HP = new int();
		public string ModelResID = "";
		public string IconResID = "";

		public void SetData(List<string> data)
		{
			Util.Convert(data[0], ref ID);
			Name = data[1];
			ClassName = data[2];
			Util.Convert(data[3], ref Price);
			Util.Convert(data[4], ref Lifespan);
			Util.Parse(data[5], ',', Properties);
			Util.Convert(data[6], ref HP);
			ModelResID = data[7];
			IconResID = data[8];
		}
	}
}

//  auto-generated file. do not modify
