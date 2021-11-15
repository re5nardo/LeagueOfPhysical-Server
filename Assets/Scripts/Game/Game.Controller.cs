﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;
using GameEvent;
using EntityMessage;
using GameFramework;
using NetworkModel.Mirror;
using Mirror;

namespace LOP
{
    public partial class Game
    {
        private void OnPlayerEnter(RoomMessage.PlayerEnter message)
        {
            var conn = message.networkConnection;
            var customProperties = conn.authenticationData as CustomProperties;

            if (IDMap.UserIdEntityId.TryGetEntityId(customProperties.userId, out var entityId))
            {
                var userEntity = Entities.Get<LOPMonoEntityBase>(entityId);

                userEntity.OwnerId = customProperties.userId;

                var enterRoom = new SC_EnterRoom();
                enterRoom.tick = Game.Current.CurrentTick;
                enterRoom.entityId = userEntity.EntityID;
                enterRoom.position = userEntity.Position;
                enterRoom.rotation = userEntity.Rotation;

                RoomNetwork.Instance.Send(enterRoom, conn.connectionId);

                //  NearEntityController
                userEntity.AttachEntityComponent<NearEntityController>();

                //  Entity Skill Info
                SkillController controller = userEntity.GetComponent<SkillController>();
                var entitySkillInfo = new SC_EntitySkillInfo();
                entitySkillInfo.entityId = userEntity.EntityID;
                entitySkillInfo.dicSkillInfo = controller.GetEntitySkillInfo();

                RoomNetwork.Instance.Send(entitySkillInfo, conn.connectionId);
            }
            else
            {
                //  Create character(Player)
                Character character = EntityHelper.CreatePlayerCharacter(customProperties.userId, customProperties.characterId);

                IDMap.UserIdEntityId.Set(customProperties.userId, character.EntityID);

                var enterRoom = new SC_EnterRoom();
                enterRoom.tick = Game.Current.CurrentTick;
                enterRoom.entityId = character.EntityID;
                enterRoom.position = character.Position;
                enterRoom.rotation = character.Rotation;

                RoomNetwork.Instance.Send(enterRoom, conn.connectionId);

                //  EntityAdditionalDatas
                CharacterGrowthData characterGrowthData = EntityAdditionalDataInitializer.Instance.Initialize(new CharacterGrowthData());
                character.AttachEntityComponent(characterGrowthData);

                EmotionExpressionData emotionExpressionData = EntityAdditionalDataInitializer.Instance.Initialize(new EmotionExpressionData(), character.EntityID);
                character.AttachEntityComponent(emotionExpressionData);

                EntityInventory entityInventory = EntityAdditionalDataInitializer.Instance.Initialize(new EntityInventory(), character.EntityID);
                character.AttachEntityComponent(entityInventory);

                character.AttachEntityComponent<NearEntityController>();

                character.AttachEntityComponent<PlayerView>();

                character.AttachEntityComponent<EntityTransformController>();
                if (character.ModelAnimator != null)
                {
                    character.AttachEntityComponent<EntityAnimatorController>();
                }

                //  Entity Skill Info
                //  (should receive data from server db?)
                SkillController controller = character.GetComponent<SkillController>();
                foreach (int nSkillID in character.MasterData.SkillIDs)
                {
                    controller.AddSkill(nSkillID);
                }

                var entitySkillInfo = new SC_EntitySkillInfo();
                entitySkillInfo.entityId = character.EntityID;
                entitySkillInfo.dicSkillInfo = controller.GetEntitySkillInfo();

                RoomNetwork.Instance.Send(entitySkillInfo, conn.connectionId);
            }
        }

        private void OnPlayerLeave(RoomMessage.PlayerLeave message)
        {
            var conn = message.networkConnection;
            var customProperties = conn.authenticationData as CustomProperties;

            IDMap.UserIdEntityId.TryGetEntityId(customProperties.userId, out var entityId);

            var entity = Entities.Get<LOPMonoEntityBase>(entityId);

            entity.OwnerId = "server";

            NearEntityController nearEntityController = entity.GetEntityComponent<NearEntityController>();
            entity.DetachEntityComponent(nearEntityController);

            Destroy(nearEntityController);
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

            character.HP += heal;

            LOP.Game.Current.GameEventManager.SendToNear(new EntityHeal(healingEntityID, healedEntityID, heal, character.HP), character.Position);
        }

        public void EntityGetMoney(int entityID, int money, Vector3 position)
        {
            var entity = Entities.Get<LOPMonoEntityBase>(entityID);

            EntityInventory entityInventory = entity.GetComponent<EntityInventory>();
            if (entityInventory != null)
            {
                entityInventory.m_nMoney += money;
            }

            if (entity.EntityRole == EntityRole.Player)
            {
                if (IDMap.TryGetConnectionIdByEntityId(entityID, out var connectionId))
                {
                    LOP.Game.Current.GameEventManager.Send(new EntityGetMoney(entityID, position, money, entityInventory.m_nMoney), connectionId);
                }
            }
        }

        public void EntityGetExp(int entityID, int exp)
        {
            var entity = Entities.Get<LOPMonoEntityBase>(entityID);

            CharacterGrowthData characterGrowthData = entity.GetComponent<CharacterGrowthData>();
            if (characterGrowthData != null)
            {
                characterGrowthData.Exp += exp;

                while (characterGrowthData.Exp >= 100)
                {
                    characterGrowthData.Level++;
                    characterGrowthData.Exp -= 100;
                }

                if (entity.EntityRole == EntityRole.Player)
                {
                    if (IDMap.TryGetConnectionIdByEntityId(entityID, out var connectionId))
                    {
                        LOP.Game.Current.GameEventManager.Send(new EntityGetExp(entityID, exp, characterGrowthData.Exp), connectionId);
                    }
                }
            }
        }

        public void AttackEntity(int nAttackerID, int nAttackedID)
        {
            int attackDamage = 0;
            int armor = 0;

            var attacker = Entities.Get<LOPMonoEntityBase>(nAttackerID);
            var attacked = Entities.Get<LOPMonoEntityBase>(nAttackedID);

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

            entity.HP = entity.HP - nDamage;

            LOP.Game.Current.GameEventManager.SendToNear(new EntityDamage(nAttackerID, nAttackedID, nDamage, entity.HP), entity.Position);

            if (entity.HP <= 0)
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

            gameItem.HP = gameItem.HP - nDamage;

            LOP.Game.Current.GameEventManager.SendToNear(new EntityDamage(nAttackerID, nAttackedID, nDamage, gameItem.HP), gameItem.Position);

            if (gameItem.HP <= 0)
            {
                //	Destroy treasure box
                DestroyEntity(nAttackedID);

                //	Create reward
                MasterData.GameItem masterData = MasterDataManager.Instance.GetMasterData<MasterData.GameItem>(Define.MasterData.GameItemID.RED_POTION);

                var item = GameItem.Builder()
                    .SetEntityId(EntityManager.Instance.GenerateEntityID())
                    .SetMasterDataId(Define.MasterData.GameItemID.RED_POTION)
                    .SetPosition(gameItem.Position)
                    .SetRotation(Vector3.zero)
                    .SetVelocity(Vector3.zero)
                    .SetAngularVelocity(Vector3.zero)
                    .SetModelId(masterData.ModelResID)
                    .SetLifespan(masterData.Lifespan)
                    .SetEntityType(EntityType.GameItem)
                    .SetEntityRole(EntityRole.NPC)
                    .SetOwnerId("server")
                    .Build();

                StateController stateController = item.GetComponent<StateController>();
                stateController.StartState(new BasicStateParam(Define.MasterData.StateId.EntitySelfDestroy, masterData.Lifespan));
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
            var entity = Entities.Get<LOPMonoEntityBase>(nEntityID);

            entity.MessageBroker.Publish(new Destroying());

            SceneMessageBroker.Publish(new GameMessage.EntityDestroy(nEntityID));

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
