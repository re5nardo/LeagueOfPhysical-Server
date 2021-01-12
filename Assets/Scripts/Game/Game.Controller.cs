using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;
using GameEvent;
using EntityCommand;
using GameFramework;
using System;

namespace LOP
{
    public partial class Game
    {
        private void OnPlayerEnter(object param)
        {
            PhotonPlayer newPlayer = (PhotonPlayer)param;

            playerUserIDPhotonPlayer[newPlayer.UserId] = new WeakReference(newPlayer);

            if (playerUserIDEntityID.ContainsKey(newPlayer.UserId))
            {
                MonoEntityBase userEntity = Entities.Get<MonoEntityBase>(playerUserIDEntityID[newPlayer.UserId]);

                SC_EnterRoom enterRoom = new SC_EnterRoom();
                enterRoom.m_nEntityID = userEntity.EntityID;
                enterRoom.m_vec3Position = userEntity.Position;
                enterRoom.m_vec3Rotation = userEntity.Rotation;
                enterRoom.m_nCurrentTick = Game.Current.CurrentTick;

                RoomNetwork.Instance.Send(enterRoom, newPlayer.ID);

                //  NearEntityController
                userEntity.AttachComponent(userEntity.gameObject.AddComponent<NearEntityController>());

                //  Entity Skill Info
                SkillController controller = userEntity.GetComponent<SkillController>();
                SC_EntitySkillInfo entitySkillInfo = new SC_EntitySkillInfo();
                entitySkillInfo.m_nEntityID = userEntity.EntityID;
                entitySkillInfo.m_dicSkillInfo = controller.GetEntitySkillInfo();

                RoomNetwork.Instance.Send(entitySkillInfo, newPlayer.ID);
            }
            else
            {
                if (!newPlayer.CustomProperties.TryGetValue("CharacterID", out object characterId))
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

                    character.AttachComponent(character.gameObject.AddComponent<NearEntityController>());
                    character.AttachComponent(character.gameObject.AddComponent<PlayerMoveInputController>());

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

        private void OnPlayerLeave(object param)
        {
            PhotonPlayer photonPlayer = (PhotonPlayer)param;

            int entityID = playerUserIDEntityID[photonPlayer.UserId];

            IEntity entity = Entities.Get(entityID);

            NearEntityController nearEntityController = entity.GetComponent<NearEntityController>();
            entity.DetachComponent(nearEntityController);

            Destroy(nearEntityController);

            playerUserIDPhotonPlayer.Remove(photonPlayer.UserId);

            //  Start AI
            //  ...

            //  m_dicPlayerUserIDEntityID 제거하는 로직 필요.. Entity destroy될 때 체크해야 할 듯?
        }

        public Rect GetMapRect()
        {
            return new Rect(new Vector2(-200, -200), new Vector2(400, 400));
        }

        public void EntityGetGameItem(int nEntityID, int nGameItemID)
        {
            GameItem gameItem = Entities.Get<GameItem>(nGameItemID);

            DestroyEntity(nGameItemID);

            Character character = Entities.Get<Character>(nEntityID);
            if (character == null || !character.IsAlive)
            {
                return;
            }

            if (gameItem.MasterData.ID == Define.MasterData.GameItemID.RED_POTION)
            {
                int heal = UnityEngine.Random.Range(50, 150);

                HealEntity(nGameItemID, nEntityID, heal);
            }
        }

        public void HealEntity(int healingEntityID, int healedEntityID, int heal)
        {
            Character character = Entities.Get<Character>(healedEntityID);
            if (character == null || !character.IsAlive)
            {
                return;
            }

            character.CurrentHP += heal;

            LOP.Game.Current.GameEventManager.SendToNear(new EntityHeal(healingEntityID, healedEntityID, heal, character.CurrentHP), character.Position);
        }

        public void EntityGetMoney(int entityID, int money, Vector3 position)
        {
            MonoEntityBase entity = Entities.Get<MonoEntityBase>(entityID);

            EntityInventory entityInventory = entity.GetComponent<EntityInventory>();
            if (entityInventory != null)
            {
                entityInventory.m_nMoney += money;
            }

            if (entity.EntityRole == EntityRole.Player)
            {
                LOP.Game.Current.GameEventManager.Send(new EntityGetMoney(entityID, position, money, entityInventory.m_nMoney), PhotonHelper.GetPhotonPlayer(entityID).ID);
            }
        }

        public void EntityGetExp(int entityID, int exp)
        {
            MonoEntityBase entity = Entities.Get<MonoEntityBase>(entityID);

            CharacterGrowthData characterGrowthData = entity.GetComponent<CharacterGrowthData>();
            if (characterGrowthData != null)
            {
                characterGrowthData.m_nExp += exp;

                while (characterGrowthData.m_nExp >= 100)
                {
                    characterGrowthData.Level++;
                    characterGrowthData.m_nExp -= 100;
                }

                if (entity.EntityRole == EntityRole.Player)
                {
                    LOP.Game.Current.GameEventManager.Send(new EntityGetExp(entityID, exp, characterGrowthData.m_nExp), PhotonHelper.GetPhotonPlayer(entityID).ID);
                }
            }
        }

        public void AttackEntity(int nAttackerID, int nAttackedID)
        {
            int attackDamage = 0;
            int armor = 0;

            MonoEntityBase attacker = Entities.Get<MonoEntityBase>(nAttackerID);
            MonoEntityBase attacked = Entities.Get<MonoEntityBase>(nAttackedID);

            //	AttackDamage
            if (attacker.EntityType == EntityType.Character)
            {
                attackDamage = (attacker as Character).SecondStatus.AttackDamage;
            }

            //	Armor
            if (attacked.EntityType == EntityType.Character)
            {
                armor = (attacker as Character).SecondStatus.Armor;
            }

            //	Damage
            int damage = attackDamage - armor;
            damage = (int)(damage * UnityEngine.Random.Range(0.8f, 1.2f));

            if (damage < 0)
            {
                Debug.LogWarning("damage : " + damage);
            }

            //	Sub process
            if (attacked.EntityType == EntityType.GameItem)
            {
                AttackGameItem(nAttackerID, nAttackedID, damage);
            }
            else if (attacked.EntityType == EntityType.Character)
            {
                AttackCharacter(nAttackerID, nAttackedID, damage);
            }
        }

        public void AttackCharacter(int nAttackerID, int nAttackedID, int nDamage)
        {
            Character entity = Entities.Get<Character>(nAttackedID);
            if (!entity.IsAlive)
                return;

            entity.CurrentHP = entity.CurrentHP - nDamage;

            LOP.Game.Current.GameEventManager.SendToNear(new EntityDamage(nAttackerID, nAttackedID, nDamage, entity.CurrentHP), entity.Position);

            if (entity.CurrentHP <= 0)
            {
                EntityDie(nAttackerID, nAttackedID);
            }

            int heal = (int)(nDamage * 0.1f * UnityEngine.Random.Range(0.8f, 1.2f));

            HealEntity(nAttackerID, nAttackerID, heal);
        }

        public void AttackGameItem(int nAttackerID, int nAttackedID, int nDamage)
        {
            GameItem gameItem = Entities.Get<GameItem>(nAttackedID);
            if (!gameItem.IsAlive || gameItem.MasterData.ID != Define.MasterData.GameItemID.TREASURE_BOX)
                return;

            gameItem.CurrentHP = gameItem.CurrentHP - nDamage;

            LOP.Game.Current.GameEventManager.SendToNear(new EntityDamage(nAttackerID, nAttackedID, nDamage, gameItem.CurrentHP), gameItem.Position);

            if (gameItem.CurrentHP <= 0)
            {
                //	Destroy treasure box
                DestroyEntity(nAttackedID);

                //	Create reward
                MasterData.GameItem masterData = MasterDataManager.Instance.GetMasterData<MasterData.GameItem>(Define.MasterData.GameItemID.RED_POTION);

                var item = GameItem.Builder()
                    .SetMasterDataID(Define.MasterData.GameItemID.RED_POTION)
                    .SetPosition(gameItem.Position)
                    .SetRotation(Vector3.zero)
                    .SetVelocity(Vector3.zero)
                    .SetAngularVelocity(Vector3.zero)
                    .SetModelPath(masterData.ModelResID)
                    .SetLifespan(masterData.Lifespan)
                    .SetEntityRole(EntityRole.NPC)
                    .Build();
            }
        }

        public void EntityDie(int nAttackerID, int nDiedID)
        {
            Character attacker = Entities.Get<Character>(nAttackerID);
            Character died = Entities.Get<Character>(nDiedID);

            BehaviorController behaviorController = died.GetComponent<BehaviorController>();
            behaviorController.Die();

            if (died.EntityRole == EntityRole.Monster)
            {
                MasterData.MonsterData monsterData = MasterDataManager.Instance.GetMasterData<MasterData.MonsterData>(0);   //  Temp
                foreach (int rewardID in monsterData.RewardIDs)
                {
                    MasterData.Reward reward = MasterDataManager.Instance.GetMasterData<MasterData.Reward>(rewardID);
                    if (reward.RewardType == "Exp")
                    {
                        if (attacker != null && attacker.EntityRole == EntityRole.Player)
                        {
                            int nRewardExp = reward.FixedQuantity ? reward.Min : UnityEngine.Random.Range(reward.Min, reward.Max + 1);

                            EntityGetExp(nAttackerID, nRewardExp);
                        }
                    }
                    else if (reward.RewardType == "Money")
                    {
                        if (attacker != null && attacker.EntityRole == EntityRole.Player)
                        {
                            int nRewardMoney = reward.FixedQuantity ? reward.Min : UnityEngine.Random.Range(reward.Min, reward.Max + 1);

                            EntityGetMoney(nAttackerID, nRewardMoney, died.Position);
                        }
                    }
                }
            }
        }

        public void DestroyEntity(int nEntityID)
        {
            MonoEntityBase entity = Entities.Get<MonoEntityBase>(nEntityID);

            entity.SendCommandToAll(new Destroying());

            GamePubSubService.Publish(GameMessageKey.EntityDestroy, new object[] { nEntityID });

            if (entity.EntityRole == EntityRole.Player)
            {
                string strPlayerUserID = entityIDPlayerUserID[entity.EntityID];

                playerUserIDEntityID.Remove(strPlayerUserID);
                entityIDPlayerUserID.Remove(entity.EntityID);
                playerUserIDPhotonPlayer.Remove(strPlayerUserID);
            }

            EntityManager.Instance.UnregisterEntity(nEntityID);

            Destroy(entity.gameObject);
        }
    }
}
