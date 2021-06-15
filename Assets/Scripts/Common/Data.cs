using System;

[Serializable]
public struct FirstStatus
{
	public FirstStatus(MasterData.FirstStatus masterData)
	{
		STR = masterData.STR;
		DEX = masterData.DEX;
		CON = masterData.CON;
		INT = masterData.INT;
		WIS = masterData.WIS;
		CHA = masterData.CHA;
	}

	public int STR;     //	근력
	public int DEX;     //	재주
	public int CON;     //	건강
	public int INT;     //	지력
	public int WIS;     //	지혜
	public int CHA;     //	매력
}

[Serializable]
public struct SecondStatus
{
	public SecondStatus(FirstStatus firstStatus, MasterData.SecondStatus secondStatusMasterData)
	{
		HP = MaximumHP = secondStatusMasterData.MaximumHP + firstStatus.CON * 15;
		HPRegen = secondStatusMasterData.HPRegen;
		AttackDamage = secondStatusMasterData.AttackDamage + firstStatus.STR * 5;
		ArmorPenetration = secondStatusMasterData.ArmorPenetration;
		HPSteal = secondStatusMasterData.HPSteal;
		AttackSpeed = secondStatusMasterData.AttackSpeed + firstStatus.DEX * 1;
		CriticalChance = secondStatusMasterData.CriticalChance;
		Range = secondStatusMasterData.Range;
		MovementSpeed = secondStatusMasterData.MovementSpeed;
		MP = MaximumMP = secondStatusMasterData.MaximumMP + firstStatus.WIS * 15;
		MPRegen = secondStatusMasterData.MPRegen;
		AbilityPower = secondStatusMasterData.AbilityPower + firstStatus.INT * 5;
		MagicPenetration = secondStatusMasterData.MagicPenetration;
		SpellVamp = secondStatusMasterData.SpellVamp;
		CooldownReduction = secondStatusMasterData.CooldownReduction;
		Armor = secondStatusMasterData.Armor;
		MagicResist = secondStatusMasterData.MagicResist;
		Tenacity = secondStatusMasterData.Tenacity;

		HIT = secondStatusMasterData.HIT;
		Dodge = secondStatusMasterData.Dodge;
		Block = secondStatusMasterData.Block;
	}

	public void Apply(FirstStatus firstStatus, MasterData.SecondStatus secondStatusMasterData)
	{
		int preMaximumHP = MaximumHP;
		int preMaximumMP = MaximumMP;

		MaximumHP = secondStatusMasterData.MaximumHP + firstStatus.CON * 15;
		AttackDamage = secondStatusMasterData.AttackDamage + firstStatus.STR * 5;
		AttackSpeed = secondStatusMasterData.AttackSpeed + firstStatus.DEX * 1;
		MaximumMP = secondStatusMasterData.MaximumMP + firstStatus.WIS * 15;
		AbilityPower = secondStatusMasterData.AbilityPower + firstStatus.INT * 5;

		HP += (MaximumHP - preMaximumHP);
		MP += (MaximumMP - preMaximumMP);
	}

	public int		HP;                     //	Hit Point
	public int		MaximumHP;              //	Maximum Hit Point
	public int		HPRegen;				//	5초당 체력 회복
	public int		AttackDamage;			//	공격력
	public float	ArmorPenetration;		//	방어구 관통력
	public float	HPSteal;				//	생명력 흡수
	public float	AttackSpeed;			//	공격 속도
	public float	CriticalChance;			//	치명타율
	public int		Range;					//	사거리
	public float	MovementSpeed;			//	이동속도
	public int		MP;                     //	Mana Point
	public int		MaximumMP;              //	Maximum Mana Point
	public int		MPRegen;				//	5초당 마나 회복
	public int		AbilityPower;			//	주문력
	public float	MagicPenetration;		//	마법 관통력
	public float	SpellVamp;				//	주문 흡혈
	public float	CooldownReduction;		//	재사용 대기시간 감소
	public int		Armor;					//	방어력
	public int		MagicResist;			//	마법 저항력
	public float	Tenacity;				//	강인함

	public float	HIT;					//	명중
	public float	Dodge;					//	회피
	public float	Block;					//	블록
}

[Serializable]
public enum EntityType
{
    None = 0,
    Character = 1,
    Projectile = 2,
	GameItem = 3,
    MapObject = 4,
}

[Serializable][Flags]
public enum EntityRole
{
    None		= 0,
    NPC			= 1 << 0,
    Monster		= 1 << 1,
    Player		= 1 << 2,

	All			= NPC | Monster | Player,
}

[Serializable]
public enum FirstStatusElement
{
	None = 0,
	STR = 1,
	DEX = 2,
	CON = 3,
	INT = 4,
	WIS = 5,
	CHA = 6,
}

[Serializable]
public enum MatchType
{
    Friendly = 0,
    Rank = 1,
}

[Serializable]
public struct MatchSetting
{
    public MatchType matchType;
    public string subGameId;
    public string mapId;

    public MatchSetting(MatchType matchType, string subGameId, string mapId)
    {
        this.matchType = matchType;
        this.subGameId = subGameId;
        this.mapId = mapId;
    }
}
