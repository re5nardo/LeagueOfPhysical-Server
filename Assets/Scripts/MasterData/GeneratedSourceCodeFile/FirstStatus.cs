using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterData
{
	public class FirstStatus : IMasterData
	{
		public int ID = new int();
		public string Name = "";
		public int STR = new int();
		public int DEX = new int();
		public int CON = new int();
		public int INT = new int();
		public int WIS = new int();
		public int CHA = new int();

		public void SetData(List<string> data)
		{
			Util.Convert(data[0], ref ID);
			Name = data[1];
			Util.Convert(data[2], ref STR);
			Util.Convert(data[3], ref DEX);
			Util.Convert(data[4], ref CON);
			Util.Convert(data[5], ref INT);
			Util.Convert(data[6], ref WIS);
			Util.Convert(data[7], ref CHA);
		}
	}
}

//  auto-generated file. do not modify
