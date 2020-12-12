using System.Collections;
using System.Collections.Generic;
using GameFramework;
using MasterData;

public class MasterDataManager : MonoSingleton<MasterDataManager>
{
    private Dictionary<System.Type, Dictionary<int, IMasterData>> m_dicMasterData = new Dictionary<System.Type, Dictionary<int, IMasterData>>();

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);

        Initialize();
    }

    private void Initialize()
    {
        SetCharacter();
        SetBehavior();
        SetState();
        SetSkill();
        SetProjectile();
        SetEmotionExpression();
        SetMonsterData();
        SetReward();
		SetGameItem();
		SetAbility();
		SetFirstStatus();
		SetSecondStatus();
	}

    private void SetCharacter()
    {
        m_dicMasterData.Add(typeof(Character), new Dictionary<int, IMasterData>());

        List<List<string>> csv = Util.ReadCSV("GeneratedCSVFile/Character");

        for (int i = 2; i < csv.Count; ++i)
        {
            Character master = new Character();
            master.SetData(csv[i]);

            m_dicMasterData[typeof(Character)].Add(master.ID, master);
        }
    }

    private void SetBehavior()
    {
        m_dicMasterData.Add(typeof(MasterData.Behavior), new Dictionary<int, IMasterData>());

        List<List<string>> csv = Util.ReadCSV("GeneratedCSVFile/Behavior");

        for (int i = 2; i < csv.Count; ++i)
        {
            MasterData.Behavior master = new MasterData.Behavior();
            master.SetData(csv[i]);

            m_dicMasterData[typeof(MasterData.Behavior)].Add(master.ID, master);
        }
    }

    private void SetState()
    {
        m_dicMasterData.Add(typeof(MasterData.State), new Dictionary<int, IMasterData>());

        List<List<string>> csv = Util.ReadCSV("GeneratedCSVFile/State");

        for (int i = 2; i < csv.Count; ++i)
        {
            MasterData.State master = new MasterData.State();
            master.SetData(csv[i]);

            m_dicMasterData[typeof(MasterData.State)].Add(master.ID, master);
        }
    }

    private void SetSkill()
    {
        m_dicMasterData.Add(typeof(MasterData.Skill), new Dictionary<int, IMasterData>());

        List<List<string>> csv = Util.ReadCSV("GeneratedCSVFile/Skill");

        for (int i = 2; i < csv.Count; ++i)
        {
            MasterData.Skill master = new MasterData.Skill();
            master.SetData(csv[i]);

            m_dicMasterData[typeof(MasterData.Skill)].Add(master.ID, master);
        }
    }

    private void SetProjectile()
    {
        m_dicMasterData.Add(typeof(MasterData.Projectile), new Dictionary<int, IMasterData>());

        List<List<string>> csv = Util.ReadCSV("GeneratedCSVFile/Projectile");

        for (int i = 2; i < csv.Count; ++i)
        {
            MasterData.Projectile master = new MasterData.Projectile();
            master.SetData(csv[i]);

            m_dicMasterData[typeof(MasterData.Projectile)].Add(master.ID, master);
        }
    }

    private void SetEmotionExpression()
    {
        m_dicMasterData.Add(typeof(MasterData.EmotionExpression), new Dictionary<int, IMasterData>());

        List<List<string>> csv = Util.ReadCSV("GeneratedCSVFile/EmotionExpression");

        for (int i = 2; i < csv.Count; ++i)
        {
            MasterData.EmotionExpression master = new MasterData.EmotionExpression();
            master.SetData(csv[i]);

            m_dicMasterData[typeof(MasterData.EmotionExpression)].Add(master.ID, master);
        }
    }

    private void SetMonsterData()
    {
        m_dicMasterData.Add(typeof(MasterData.MonsterData), new Dictionary<int, IMasterData>());

        List<List<string>> csv = Util.ReadCSV("GeneratedCSVFile/MonsterData");

        for (int i = 2; i < csv.Count; ++i)
        {
            MasterData.MonsterData master = new MasterData.MonsterData();
            master.SetData(csv[i]);

            m_dicMasterData[typeof(MasterData.MonsterData)].Add(master.ID, master);
        }
    }

    private void SetReward()
    {
        m_dicMasterData.Add(typeof(MasterData.Reward), new Dictionary<int, IMasterData>());

        List<List<string>> csv = Util.ReadCSV("GeneratedCSVFile/Reward");

        for (int i = 2; i < csv.Count; ++i)
        {
            MasterData.Reward master = new MasterData.Reward();
            master.SetData(csv[i]);

            m_dicMasterData[typeof(MasterData.Reward)].Add(master.ID, master);
        }
    }

	private void SetGameItem()
	{
		m_dicMasterData.Add(typeof(MasterData.GameItem), new Dictionary<int, IMasterData>());

		List<List<string>> csv = Util.ReadCSV("GeneratedCSVFile/GameItem");

		for (int i = 2; i < csv.Count; ++i)
		{
			MasterData.GameItem master = new MasterData.GameItem();
			master.SetData(csv[i]);

			m_dicMasterData[typeof(MasterData.GameItem)].Add(master.ID, master);
		}
	}

	private void SetAbility()
	{
		m_dicMasterData.Add(typeof(MasterData.Ability), new Dictionary<int, IMasterData>());

		List<List<string>> csv = Util.ReadCSV("GeneratedCSVFile/Ability");

		for (int i = 2; i < csv.Count; ++i)
		{
			MasterData.Ability master = new MasterData.Ability();
			master.SetData(csv[i]);

			m_dicMasterData[typeof(MasterData.Ability)].Add(master.ID, master);
		}
	}

	private void SetFirstStatus()
	{
		m_dicMasterData.Add(typeof(MasterData.FirstStatus), new Dictionary<int, IMasterData>());

		List<List<string>> csv = Util.ReadCSV("GeneratedCSVFile/FirstStatus");

		for (int i = 2; i < csv.Count; ++i)
		{
			MasterData.FirstStatus master = new MasterData.FirstStatus();
			master.SetData(csv[i]);

			m_dicMasterData[typeof(MasterData.FirstStatus)].Add(master.ID, master);
		}
	}

	private void SetSecondStatus()
	{
		m_dicMasterData.Add(typeof(MasterData.SecondStatus), new Dictionary<int, IMasterData>());

		List<List<string>> csv = Util.ReadCSV("GeneratedCSVFile/SecondStatus");

		for (int i = 2; i < csv.Count; ++i)
		{
			MasterData.SecondStatus master = new MasterData.SecondStatus();
			master.SetData(csv[i]);

			m_dicMasterData[typeof(MasterData.SecondStatus)].Add(master.ID, master);
		}
	}

	public T GetMasterData<T>(int nKey) where T : IMasterData
    {
        return (T)m_dicMasterData[typeof(T)][nKey];
    }
}
