using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using Entity;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using System.Collections.ObjectModel;

namespace LOP
{
    public partial class Game : GameFramework.Game
    {
        public const float BROADCAST_SCOPE = 80f;
        public const float BROADCAST_SCOPE_RADIUS = BROADCAST_SCOPE * 0.5f;

        public new static Game Current { get { return GameFramework.Game.Current as Game; } }

        private Dictionary<string, int> playerUserIDEntityID = new Dictionary<string, int>();                                  //  key : Player UserID, vlue : EntityID
        private Dictionary<int, string> entityIDPlayerUserID = new Dictionary<int, string>();                                  //  key : EntityID, vlue : Player UserID
        private Dictionary<string, WeakReference> playerUserIDPhotonPlayer = new Dictionary<string, WeakReference>();          //  key : Player UserID, vlue : PhotonPlayer

        public ReadOnlyDictionary<string, int> PlayerUserIDEntityID { get; private set; }
        public ReadOnlyDictionary<int, string> EntityIDPlayerUserID { get; private set; }
        public ReadOnlyDictionary<string, WeakReference> PlayerUserIDPhotonPlayer { get; private set; }

        private GameEventManager gameEventManager;
        public GameEventManager GameEventManager { get { return gameEventManager; } }

        private GameProtocolDispatcher protocolDispatcher = null;
        private PlayerMoveInputManager playerMoveInputManager = null;
        private RoomPubMessageHandler roomPubMessageHandler = null;

        public override IEnumerator Initialize()
        {
            yield return SceneManager.LoadSceneAsync("RiftOfSummoner", LoadSceneMode.Additive);

            Physics.autoSimulation = false;

            PlayerUserIDEntityID = new ReadOnlyDictionary<string, int>(playerUserIDEntityID);
            EntityIDPlayerUserID = new ReadOnlyDictionary<int, string>(entityIDPlayerUserID);
            PlayerUserIDPhotonPlayer = new ReadOnlyDictionary<string, WeakReference>(playerUserIDPhotonPlayer);

            ResourcePool.Instantiate();

            protocolDispatcher = gameObject.AddComponent<GameProtocolDispatcher>();
            playerMoveInputManager = gameObject.AddComponent<PlayerMoveInputManager>();

            tickUpdater = gameObject.AddComponent<TickUpdater>();
            gameEventManager = gameObject.AddComponent<GameEventManager>();

            RoomNetwork.Instance.onMessage += OnNetworkMessage;

            roomPubMessageHandler = gameObject.AddComponent<RoomPubMessageHandler>();
            roomPubMessageHandler.Initialize(new Dictionary<Enum, Action<object[]>>
            {
                {RoomMessageKey.PlayerEnter, OnPlayerEnter},
                {RoomMessageKey.PlayerLeave, OnPlayerLeave},
            });

            tickUpdater.Initialize(1 / 30f, false, OnTick, OnTickEnd);
        }

        protected override void Clear()
        {
            base.Clear();

            playerUserIDEntityID = null;
            entityIDPlayerUserID = null;
            playerUserIDPhotonPlayer = null;

            if (protocolDispatcher != null)
            {
                Destroy(protocolDispatcher);
                protocolDispatcher = null;
            }

            if (playerMoveInputManager != null)
            {
                Destroy(playerMoveInputManager);
                playerMoveInputManager = null;
            }

            if (tickUpdater != null)
            {
                Destroy(tickUpdater);
                tickUpdater = null;
            }

            if (gameEventManager != null)
            {
                Destroy(gameEventManager);
                gameEventManager = null;
            }

            if (roomPubMessageHandler != null)
            {
                Destroy(roomPubMessageHandler);
                roomPubMessageHandler = null;
            }

            if (RoomNetwork.IsInstantiated())
            {
                RoomNetwork.Instance.onMessage -= OnNetworkMessage;
            }
        }

        protected override void OnBeforeRun()
        {
            SpawnManager.Instance.StartSpawn();

            InvokeRepeating("SendSyncTick", 0f, 0.1f);
        }

        private void OnTick(int tick)
        {
            var entities = EntityManager.Instance.GetAllEntities().Cast<MonoEntityBase>().ToList();

            //  sort
            //  ...

            entities.ForEach(entity =>
            {
                //  Iterating중에 Entity가 Destroy 안되었는지 체크
                if (entity.IsValid)
                {
                    entity.Tick(tick);
                }
            });

            entities.ForEach(entity =>
            {
                if (entity.IsValid)
                {
                    entity.OnBeforePhysicsSimulation(tick);
                }
            });

            Physics.Simulate(TickInterval);

            entities.ForEach(entity =>
            {
                if (entity.IsValid)
                {
                    entity.OnAfterPhysicsSimulation(tick);
                }
            });

            EntityInfoSender.Instance.Tick(tick);
            SpawnManager.Instance.Tick(tick);
        }

        private void OnTickEnd(int tick)
        {
            //BroadCastGameEvent();

            //gameEvents.Clear();
        }

        private void SendSyncTick()
        {
            RoomNetwork.Instance.SendToAll(new SC_SyncTick(CurrentTick));
        }

        private void OnNetworkMessage(IMessage msg, object[] objects)
        {
            protocolDispatcher.DispatchProtocol(msg as IPhotonEventMessage);
        }

        private void OnPlayerEnter(object[] param)
        {
            PhotonPlayer newPlayer = (PhotonPlayer)param[0];

            playerUserIDPhotonPlayer[newPlayer.UserId] = new WeakReference(newPlayer);

            if (playerUserIDEntityID.ContainsKey(newPlayer.UserId))
            {
                MonoEntityBase userEntity = EntityManager.Instance.GetEntity(playerUserIDEntityID[newPlayer.UserId]) as MonoEntityBase;

                SC_EnterRoom enterRoom = new SC_EnterRoom();
                enterRoom.m_nEntityID = userEntity.EntityID;
                enterRoom.m_vec3Position = userEntity.Position;
                enterRoom.m_vec3Rotation = userEntity.Rotation;
                enterRoom.m_nCurrentTick = Game.Current.CurrentTick;

                RoomNetwork.Instance.Send(enterRoom, newPlayer.ID);

                //  NearEntityAgent
                userEntity.AttachComponent(userEntity.gameObject.AddComponent<NearEntityAgent>());

                //  Entity Skill Info
                SkillController controller = userEntity.GetComponent<SkillController>();
                SC_EntitySkillInfo entitySkillInfo = new SC_EntitySkillInfo();
                entitySkillInfo.m_nEntityID = userEntity.EntityID;
                entitySkillInfo.m_dicSkillInfo = controller.GetEntitySkillInfo();

                RoomNetwork.Instance.Send(entitySkillInfo, newPlayer.ID);
            }
            else
            {
                object characterId;
                if (!newPlayer.CustomProperties.TryGetValue("CharacterID", out characterId))
                {
                    Debug.LogError("CharacterID does not exist!");
                }
                else
                {
                    //  Create character(Player)
                    Character character = EntityHelper.CreatePlayerCharacter((int)characterId);

                    playerUserIDEntityID.Add(newPlayer.UserId, character.EntityID);
                    entityIDPlayerUserID.Add(character.EntityID, newPlayer.UserId);

                    SC_EnterRoom enterRoom = new SC_EnterRoom();
                    enterRoom.m_nEntityID = character.EntityID;
                    enterRoom.m_vec3Position = character.Position;
                    enterRoom.m_vec3Rotation = character.Rotation;
                    enterRoom.m_nCurrentTick = Game.Current.CurrentTick;

                    RoomNetwork.Instance.Send(enterRoom, newPlayer.ID);

                    //  EntityAdditionalDatas
                    CharacterGrowthData characterGrowthData = EntityAdditionalDataInitializer.Instance.Initialize(new CharacterGrowthData());
                    character.AttachComponent(characterGrowthData);

                    EmotionExpressionData emotionExpressionData = EntityAdditionalDataInitializer.Instance.Initialize(new EmotionExpressionData(), character.EntityID);
                    character.AttachComponent(emotionExpressionData);

                    EntityInventory entityInventory = EntityAdditionalDataInitializer.Instance.Initialize(new EntityInventory(), character.EntityID);
                    character.AttachComponent(entityInventory);

                    character.AttachComponent(character.gameObject.AddComponent<NearEntityAgent>());

                    character.AttachComponent(character.gameObject.AddComponent<PlayerView>());

                    //  Entity Skill Info
                    //  (should receive data from server db?)
                    SkillController controller = character.GetComponent<SkillController>();
                    foreach (int nSkillID in character.MasterData.SkillIDs)
                    {
                        controller.AddSkill(nSkillID);
                    }

                    SC_EntitySkillInfo entitySkillInfo = new SC_EntitySkillInfo();
                    entitySkillInfo.m_nEntityID = character.EntityID;
                    entitySkillInfo.m_dicSkillInfo = controller.GetEntitySkillInfo();

                    RoomNetwork.Instance.Send(entitySkillInfo, newPlayer.ID);
                }
            }
        }

        private void OnPlayerLeave(object[] param)
        {
            PhotonPlayer photonPlayer = (PhotonPlayer)param[0];

            int entityID = playerUserIDEntityID[photonPlayer.UserId];

            IEntity entity = EntityManager.Instance.GetEntity(entityID);

            NearEntityAgent nearEntityAgent = entity.GetComponent<NearEntityAgent>();
            entity.DetachComponent(nearEntityAgent);

            Destroy(nearEntityAgent);

            playerUserIDPhotonPlayer.Remove(photonPlayer.UserId);

            //  Start AI
            //  ...

            //  m_dicPlayerUserIDEntityID 제거하는 로직 필요.. Entity destroy될 때 체크해야 할 듯?
        }
    }
}
