using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterData
{
	public class Skill : IMasterData
	{
		public int ID = new int();
		public string Name = "";
		public string ClassName = "";
		public List<string> ClassParams = new List<string>();
		public List<string> Properties = new List<string>();
		public string SkillType = "";
		public float CoolTime = new float();
		public float MP = new float();
		public string IconResID = "";

		public void SetData(List<string> data)
		{
			Util.Convert(data[0], ref ID);
			Name = data[1];
			ClassName = data[2];
			Util.Parse(data[3], ',', ClassParams);
			Util.Parse(data[4], ',', Properties);
			SkillType = data[5];
			Util.Convert(data[6], ref CoolTime);
			Util.Convert(data[7], ref MP);
			IconResID = data[8];
		}
	}
}

//  auto-generated file. do not modify
