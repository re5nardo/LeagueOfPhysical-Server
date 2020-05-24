using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterData
{
	public class MonsterData : IMasterData
	{
		public int ID = new int();
		public string Name = "";
		public string EntityType = "";
		public int TargetID = new int();
		public List<int> RewardIDs = new List<int>();

		public void SetData(List<string> data)
		{
			Util.Convert(data[0], ref ID);
			Name = data[1];
			EntityType = data[2];
			Util.Convert(data[3], ref TargetID);
			Util.Parse(data[4], ',', RewardIDs);
		}
	}
}

//  auto-generated file. do not modify
