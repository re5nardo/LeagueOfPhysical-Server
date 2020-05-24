using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterData
{
	public class Ability : IMasterData
	{
		public int ID = new int();
		public string Name = "";
		public string ClassName = "";
		public int Rarity = new int();
		public string IconResID = "";

		public void SetData(List<string> data)
		{
			Util.Convert(data[0], ref ID);
			Name = data[1];
			ClassName = data[2];
			Util.Convert(data[3], ref Rarity);
			IconResID = data[4];
		}
	}
}

//  auto-generated file. do not modify
