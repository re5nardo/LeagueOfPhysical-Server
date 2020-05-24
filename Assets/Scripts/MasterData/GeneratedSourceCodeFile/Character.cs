using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterData
{
	public class Character : IMasterData
	{
		public int ID = new int();
		public string Name = "";
		public string ClassName = "";
		public List<int> SkillIDs = new List<int>();
		public int FirstStatusID = new int();
		public int SecondStatusID = new int();
		public string ModelResID = "";

		public void SetData(List<string> data)
		{
			Util.Convert(data[0], ref ID);
			Name = data[1];
			ClassName = data[2];
			Util.Parse(data[3], ',', SkillIDs);
			Util.Convert(data[4], ref FirstStatusID);
			Util.Convert(data[5], ref SecondStatusID);
			ModelResID = data[6];
		}
	}
}

//  auto-generated file. do not modify
