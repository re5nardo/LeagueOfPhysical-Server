using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterData
{
	public class EmotionExpression : IMasterData
	{
		public int ID = new int();
		public string Name = "";
		public string ResID = "";

		public void SetData(List<string> data)
		{
			Util.Convert(data[0], ref ID);
			Name = data[1];
			ResID = data[2];
		}
	}
}

//  auto-generated file. do not modify
