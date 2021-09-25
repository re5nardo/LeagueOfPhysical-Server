using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;
using GameFramework;

public class SpawnManager : MonoBehaviour
{
    private const float SPAWN_INTERVAL = 0.3f;
    private const int MAX_ENTITY_COUNT = 500;

    private float m_fSpawnElapsedTime = 0f;
    private Dictionary<int, ControllerBase> m_dicEntityIDController = new Dictionary<int, ControllerBase>();

	private void Awake()
    {
        SceneMessageBroker.AddSubscriber<GameMessage.EntityDestroy>(OnEntityDestroy);
        SceneMessageBroker.AddSubscriber<TickMessage.Tick>(OnTick);
	}

    private void OnDestroy()
    {
        SceneMessageBroker.RemoveSubscriber<GameMessage.EntityDestroy>(OnEntityDestroy);
        SceneMessageBroker.RemoveSubscriber<TickMessage.Tick>(OnTick);
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

    private void OnTick(TickMessage.Tick message)
    {
        if (m_fSpawnElapsedTime > SPAWN_INTERVAL && MAX_ENTITY_COUNT > Entities.AllIDs.Count)
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
		int characterID = UnityEngine.Random.Range(Define.MasterData.CharacterID.EVELYNN, Define.MasterData.CharacterID.MALPHITE + 1);
		MasterData.Character characterMasterData = MasterDataManager.Instance.GetMasterData<MasterData.Character>(characterID);
		MasterData.FirstStatus firstStatusMasterData = MasterDataManager.Instance.GetMasterData<MasterData.FirstStatus>(characterMasterData.FirstStatusID);
		MasterData.SecondStatus secondStatusMasterData = MasterDataManager.Instance.GetMasterData<MasterData.SecondStatus>(characterMasterData.SecondStatusID);

        Rect mapRect = LOP.Game.Current.GetMapRect();
        Vector3 vec3StartPosition = new Vector3(UnityEngine.Random.Range(mapRect.xMin + 5, mapRect.xMax - 5), 0, UnityEngine.Random.Range(mapRect.yMin + 5, mapRect.yMax - 5));
        Vector3 vec3StartRotation = new Vector3(0, UnityEngine.Random.Range(0, 360), 0);

		FirstStatus firstStatus = new FirstStatus(firstStatusMasterData);

		var monster = Character.Builder()
            .SetEntityId(EntityManager.Instance.GenerateEntityID())
            .SetMasterDataId(characterID)
            .SetPosition(vec3StartPosition)
            .SetRotation(vec3StartRotation)
			.SetVelocity(Vector3.zero)
			.SetAngularVelocity(Vector3.zero)
			.SetModelId(characterMasterData.ModelResID)
            .SetFirstStatus(firstStatus)
			.SetSecondStatus(new SecondStatus(firstStatus, secondStatusMasterData))
            .SetEntityType(EntityType.Character)
            .SetEntityRole(EntityRole.Monster)
            .SetOwnerId("server")
            .Build();

		return monster;
	}

	private GameItem CreateTreasureBox()
	{
		MasterData.GameItem masterData = MasterDataManager.Instance.GetMasterData<MasterData.GameItem>(Define.MasterData.GameItemID.TREASURE_BOX);

		Rect mapRect = LOP.Game.Current.GetMapRect();
		Vector3 vec3StartPosition = new Vector3(UnityEngine.Random.Range(mapRect.xMin + 5, mapRect.xMax - 5), 0, UnityEngine.Random.Range(mapRect.yMin + 5, mapRect.yMax - 5));
		Vector3 vec3StartRotation = new Vector3(0, UnityEngine.Random.Range(0, 360), 0);

		var treasureBox = GameItem.Builder()
            .SetEntityId(EntityManager.Instance.GenerateEntityID())
            .SetMasterDataId(Define.MasterData.GameItemID.TREASURE_BOX)
			.SetPosition(vec3StartPosition)
			.SetRotation(vec3StartRotation)
			.SetVelocity(Vector3.zero)
			.SetAngularVelocity(Vector3.zero)
			.SetModelId(masterData.ModelResID)
			.SetLifespan(masterData.Lifespan)
            .SetEntityType(EntityType.GameItem)
            .SetEntityRole(EntityRole.NPC)
            .SetOwnerId("server")
            .Build();

        StateController stateController = treasureBox.GetComponent<StateController>();
        stateController.StartState(new BasicStateParam(Define.MasterData.StateID.EntitySelfDestroy, masterData.Lifespan));

        return treasureBox;
	}

	#region Message Handler
	private void OnEntityDestroy(GameMessage.EntityDestroy message)
	{
		if (m_dicEntityIDController.ContainsKey(message.entityId))
		{
			ControllerBase ai = m_dicEntityIDController[message.entityId];

			ai.UnPossess();

			Destroy(ai.gameObject);

			m_dicEntityIDController.Remove(message.entityId);
		}
	}
	#endregion
}
