using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;
using System;
using GameFramework;

public class SpawnManager : MonoSingleton<SpawnManager>, ISubscriber, ITickable
{
    private const float SPAWN_INTERVAL = 0.3f;
    private const int MAX_ENTITY_COUNT = 500;

    private float m_fSpawnElapsedTime = 0f;
    private Dictionary<int, ControllerBase> m_dicEntityIDController = new Dictionary<int, ControllerBase>();

	private Dictionary<Enum, Action<object[]>> m_dicMessageHandler = new Dictionary<Enum, Action<object[]>>();

	protected override void Awake()
    {
        base.Awake();

        GamePubSubService.Instance.AddSubscriber(GameMessageKey.EntityDestroy, this);

		m_dicMessageHandler.Add(GameMessageKey.EntityDestroy, OnEntityDestroy);
	}

    private void OnDestroy()
    {
        if (GamePubSubService.IsInstantiated())
        {
            GamePubSubService.Instance.RemoveSubscriber(GameMessageKey.EntityDestroy, this);
        }

		m_dicMessageHandler.Clear();
	}

    public void StartSpawn()
    {
		for (int i = 0; i < 200; ++i)
		{
			Character monster = CreateMonster();
			
			AINecromancerControllerAsMonster ai = new GameObject("AINecromancerController", typeof(AINecromancerControllerAsMonster)).GetComponent<AINecromancerControllerAsMonster>();

			ai.Possess(monster);

			m_dicEntityIDController.Add(monster.EntityID, ai);
		}

		for (int i = 0; i < 100; ++i)
		{
			CreateTreasureBox();
		}
	}

    public void Tick(int tick)
    {
        if (m_fSpawnElapsedTime > SPAWN_INTERVAL && MAX_ENTITY_COUNT > EntityManager.Instance.GetAllEntities().Count)
        {
            //  Create Monster
            Character monster = CreateMonster();

            AINecromancerControllerAsMonster ai = new GameObject("AINecromancerController", typeof(AINecromancerControllerAsMonster)).GetComponent<AINecromancerControllerAsMonster>();

            ai.Possess(monster);

            m_dicEntityIDController.Add(monster.EntityID, ai);

            m_fSpawnElapsedTime = 0f;
        }
        else
        {
            m_fSpawnElapsedTime += Game.Current.TickInterval;
        }
    }

    private Character CreateMonster()
    {
		int characterID = UnityEngine.Random.Range(MasterDataDefine.CharacterID.EVELYNN, MasterDataDefine.CharacterID.MALPHITE + 1);
		MasterData.Character characterMasterData = MasterDataManager.Instance.GetMasterData<MasterData.Character>(characterID);
		MasterData.FirstStatus firstStatusMasterData = MasterDataManager.Instance.GetMasterData<MasterData.FirstStatus>(characterMasterData.FirstStatusID);
		MasterData.SecondStatus secondStatusMasterData = MasterDataManager.Instance.GetMasterData<MasterData.SecondStatus>(characterMasterData.SecondStatusID);

        Rect mapRect = LOP.Game.Current.GetMapRect();
        Vector3 vec3StartPosition = new Vector3(UnityEngine.Random.Range(mapRect.xMin + 5, mapRect.xMax - 5), 0, UnityEngine.Random.Range(mapRect.yMin + 5, mapRect.yMax - 5));
        Vector3 vec3StartRotation = new Vector3(0, UnityEngine.Random.Range(0, 360), 0);

		FirstStatus firstStatus = new FirstStatus(firstStatusMasterData);

		var monster = Character.Builder()
            .SetMasterDataID(characterID)
            .SetPosition(vec3StartPosition)
            .SetRotation(vec3StartRotation)
			.SetVelocity(Vector3.zero)
			.SetAngularVelocity(Vector3.zero)
			.SetModelPath(characterMasterData.ModelResID)
            .SetFirstStatus(firstStatus)
			.SetSecondStatus(new SecondStatus(firstStatus, secondStatusMasterData))
			.SetSelectableFirstStatusCount(0)
            .SetEntityRole(EntityRole.Monster)
			.Build();

		return monster;
	}

	private GameItem CreateTreasureBox()
	{
		MasterData.GameItem masterData = MasterDataManager.Instance.GetMasterData<MasterData.GameItem>(MasterDataDefine.GameItem.TREASURE_BOX);

		Rect mapRect = LOP.Game.Current.GetMapRect();
		Vector3 vec3StartPosition = new Vector3(UnityEngine.Random.Range(mapRect.xMin + 5, mapRect.xMax - 5), 0, UnityEngine.Random.Range(mapRect.yMin + 5, mapRect.yMax - 5));
		Vector3 vec3StartRotation = new Vector3(0, UnityEngine.Random.Range(0, 360), 0);

		var treasureBox = GameItem.Builder()
			.SetMasterDataID(MasterDataDefine.GameItem.TREASURE_BOX)
			.SetPosition(vec3StartPosition)
			.SetRotation(vec3StartRotation)
			.SetVelocity(Vector3.zero)
			.SetAngularVelocity(Vector3.zero)
			.SetModelPath(masterData.ModelResID)
			.SetLifespan(masterData.Lifespan)
            .SetEntityRole(EntityRole.NPC)
            .Build();

		return treasureBox;
	}

	#region ISubscriber
    public void OnMessage(Enum key, params object[] param)
    {
		m_dicMessageHandler[key](param);
	}
	#endregion

	#region Message Handler
	private void OnEntityDestroy(params object[] param)
	{
		int nEntityID = (int)param[0];

		if (m_dicEntityIDController.ContainsKey(nEntityID))
		{
			ControllerBase ai = m_dicEntityIDController[nEntityID];

			ai.UnPossess();

			Destroy(ai.gameObject);

			m_dicEntityIDController.Remove(nEntityID);
		}
	}
	#endregion
}
