using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterData
{
	public class SecondStatus : IMasterData
	{
		public int ID = new int();
		public string Name = "";
		public int MaximumHP = new int();
		public int HPRegen = new int();
		public int AttackDamage = new int();
		public float ArmorPenetration = new float();
		public float HPSteal = new float();
		public float AttackSpeed = new float();
		public float CriticalChance = new float();
		public int Range = new int();
		public float MovementSpeed = new float();
		public int MaximumMP = new int();
		public int MPRegen = new int();
		public int AbilityPower = new int();
		public float MagicPenetration = new float();
		public float SpellVamp = new float();
		public float CooldownReduction = new float();
		public int Armor = new int();
		public int MagicResist = new int();
		public float Tenacity = new float();
		public float HIT = new float();
		public float Dodge = new float();
		public float Block = new float();

		public void SetData(List<string> data)
		{
			Util.Convert(data[0], ref ID);
			Name = data[1];
			Util.Convert(data[2], ref MaximumHP);
			Util.Convert(data[3], ref HPRegen);
			Util.Convert(data[4], ref AttackDamage);
			Util.Convert(data[5], ref ArmorPenetration);
			Util.Convert(data[6], ref HPSteal);
			Util.Convert(data[7], ref AttackSpeed);
			Util.Convert(data[8], ref CriticalChance);
			Util.Convert(data[9], ref Range);
			Util.Convert(data[10], ref MovementSpeed);
			Util.Convert(data[11], ref MaximumMP);
			Util.Convert(data[12], ref MPRegen);
			Util.Convert(data[13], ref AbilityPower);
			Util.Convert(data[14], ref MagicPenetration);
			Util.Convert(data[15], ref SpellVamp);
			Util.Convert(data[16], ref CooldownReduction);
			Util.Convert(data[17], ref Armor);
			Util.Convert(data[18], ref MagicResist);
			Util.Convert(data[19], ref Tenacity);
			Util.Convert(data[20], ref HIT);
			Util.Convert(data[21], ref Dodge);
			Util.Convert(data[22], ref Block);
		}
	}
}

//  auto-generated file. do not modify
