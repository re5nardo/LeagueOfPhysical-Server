using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Entity;
using UnityEngine.SceneManagement;
using System;
using System.Collections.ObjectModel;
using GameFramework;

namespace LOP
{
    public class Room : PunBehaviour, ISubscriber
    {
        private Dictionary<string, int> m_dicPlayerUserIDEntityID = new Dictionary<string, int>();                                  //  key : Player UserID, vlue : EntityID
        private Dictionary<int, string> m_dicEntityIDPlayerUserID = new Dictionary<int, string>();                                  //  key : EntityID, vlue : Player UserID
        private Dictionary<string, WeakReference> m_dicPlayerUserIDPhotonPlayer = new Dictionary<string, WeakReference>();          //  key : Player UserID, vlue : PhotonPlayer

        public ReadOnlyDictionary<string, int> dicPlayerUserIDEntityID { get; private set; }
        public ReadOnlyDictionary<int, string> dicEntityIDPlayerUserID { get; private set; }
        public ReadOnlyDictionary<string, WeakReference> dicPlayerUserIDPhotonPlayer { get; private set; }

        private Game game = null;
        private RoomProtocolDispatcher protocolDispatcher = null;

        private Dictionary<Enum, Action<object[]>> m_dicMessageHandler = new Dictionary<Enum, Action<object[]>>();

        public static Room Instance { get; private set; }

        public static bool IsInstantiated()
        {
            return Instance != null;
        }

        #region MonoBehaviour
        private void Awake()
        {
            Instance = this;

            Init();
        }

        private IEnumerator Start()
        {
            yield return SceneManager.LoadSceneAsync("RiftOfSummoner", LoadSceneMode.Additive);

            Physics.autoSimulation = false;

            game.Run();

            InvokeRepeating("SyncTick", 0f, 0.1f);

            ResourcePool.Instantiate();

            PhotonNetwork.isMessageQueueRunning = true;
        }

        private void OnDestroy()
        {
            Clear();

            Instance = null;
        }
        #endregion

        #region PunBehaviour
        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            PrepareEnterRoom(newPlayer);
        }

        private void PrepareEnterRoom(PhotonPlayer newPlayer)
        {
            m_dicPlayerUserIDPhotonPlayer[newPlayer.UserId] = new WeakReference(newPlayer);

            if (m_dicPlayerUserIDEntityID.ContainsKey(newPlayer.UserId))
            {
                MonoEntityBase userEntity = EntityManager.Instance.GetEntity(m_dicPlayerUserIDEntityID[newPlayer.UserId]) as MonoEntityBase;

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

                    m_dicPlayerUserIDEntityID.Add(newPlayer.UserId, character.EntityID);
                    m_dicEntityIDPlayerUserID.Add(character.EntityID, newPlayer.UserId);

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

                    EntityInventoryData entityInventoryData = EntityAdditionalDataInitializer.Instance.Initialize(new EntityInventoryData(), character.EntityID);
                    character.AttachComponent(entityInventoryData);

                    character.AttachComponent(character.gameObject.AddComponent<NearEntityAgent>());

                    character.AttachComponent(character.gameObject.AddComponent<PlayerView>());

                    //  Entity Skill Info
                    //  (should receive data from server db?)
                    SkillController controller = character.GetComponent<SkillController>();
                    foreach (int nSkillID in character.m_MasterData.SkillIDs)
                    {
                        controller.AddSkill(nSkillID);
                    }

                    SC_EntitySkillInfo entitySkillInfo = new SC_EntitySkillInfo();
                    entitySkillInfo.m_nEntityID = character.EntityID;
                    entitySkillInfo.m_dicSkillInfo = controller.GetEntitySkillInfo();

                    RoomNetwork.Instance.Send(entitySkillInfo, newPlayer.ID);
                }
            }

            RoomPubSubService.Instance.Publish(MessageKey.PlayerEnter, m_dicPlayerUserIDEntityID[newPlayer.UserId]);
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            int entityID = m_dicPlayerUserIDEntityID[otherPlayer.UserId];

            IEntity entity = EntityManager.Instance.GetEntity(entityID);

            NearEntityAgent nearEntityAgent = entity.GetComponent<NearEntityAgent>();
            entity.DetachComponent(nearEntityAgent);

            Destroy(nearEntityAgent);

            m_dicPlayerUserIDPhotonPlayer.Remove(otherPlayer.UserId);

            RoomPubSubService.Instance.Publish(MessageKey.PlayerLeave, entityID);

            //  Start AI
            //  ...

            //  m_dicPlayerUserIDEntityID 제거하는 로직 필요.. Entity destroy될 때 체크해야 할 듯?
        }
        #endregion

        private void Init()
        {
            MasterDataManager.Instance.Initialize();

            dicPlayerUserIDEntityID = new ReadOnlyDictionary<string, int>(m_dicPlayerUserIDEntityID);
            dicEntityIDPlayerUserID = new ReadOnlyDictionary<int, string>(m_dicEntityIDPlayerUserID);
            dicPlayerUserIDPhotonPlayer = new ReadOnlyDictionary<string, WeakReference>(m_dicPlayerUserIDPhotonPlayer);

            game = gameObject.AddComponent<Game>();
            game.Initialize();

            protocolDispatcher = gameObject.AddComponent<RoomProtocolDispatcher>();
            gameObject.AddComponent<PlayerMoveInputManager>();

            RoomNetwork.Instance.onMessage += OnNetworkMessage;

            RoomPubSubService.Instance.AddSubscriber(MessageKey.EntityDestroy, this);

            m_dicMessageHandler.Add(MessageKey.EntityDestroy, OnEntityDestroy);
        }

        private void Clear()
        {
            dicPlayerUserIDEntityID = null;
            dicEntityIDPlayerUserID = null;
            dicPlayerUserIDPhotonPlayer = null;

            game = null;
            protocolDispatcher = null;

            if (RoomNetwork.IsInstantiated())
            {
                RoomNetwork.Instance.onMessage -= OnNetworkMessage;
            }

            if (RoomPubSubService.IsInstantiated())
            {
                RoomPubSubService.Instance.RemoveSubscriber(MessageKey.EntityDestroy, this);
            }

            m_dicMessageHandler.Clear();
        }

        private void SyncTick()
        {
            RoomNetwork.Instance.SendToAll(new SC_SyncTick(game.CurrentTick));
        }

        private void OnNetworkMessage(IMessage msg, object[] objects)
        {
            protocolDispatcher.DispatchProtocol(msg as IPhotonEventMessage);
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

            MonoEntityBase entity = EntityManager.Instance.GetEntity(nEntityID) as MonoEntityBase;

            if (entity.EntityRole == EntityRole.Player)
            {
                string strPlayerUserID = m_dicEntityIDPlayerUserID[entity.EntityID];

                m_dicPlayerUserIDEntityID.Remove(strPlayerUserID);
                m_dicEntityIDPlayerUserID.Remove(entity.EntityID);
                m_dicPlayerUserIDPhotonPlayer.Remove(strPlayerUserID);
            }
        }
        #endregion
    }

}
