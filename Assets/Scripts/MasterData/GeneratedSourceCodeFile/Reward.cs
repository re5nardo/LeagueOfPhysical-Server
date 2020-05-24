using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterData
{
	public class Reward : IMasterData
	{
		public int ID = new int();
		public string Name = "";
		public string RewardType = "";
		public bool FixedQuantity = new bool();
		public int Min = new int();
		public int Max = new int();

		public void SetData(List<string> data)
		{
			Util.Convert(data[0], ref ID);
			Name = data[1];
			RewardType = data[2];
			Util.Convert(data[4], ref Min);
			Util.Convert(data[5], ref Max);
		}
	}
}

//  auto-generated file. do not modify
