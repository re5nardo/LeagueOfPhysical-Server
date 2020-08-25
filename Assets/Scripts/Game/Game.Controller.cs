using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;
using GameEvent;
using EntityCommand;

namespace LOP
{
    public partial class Game
    {
        public Rect GetMapRect()
        {
            return new Rect(new Vector2(-200, -200), new Vector2(400, 400));
        }

        public void EntityGetGameItem(int nEntityID, int nGameItemID)
        {
            GameItem gameItem = EntityManager.Instance.GetEntity(nGameItemID) as GameItem;

            DestroyEntity(nGameItemID);

            Character character = EntityManager.Instance.GetEntity(nEntityID) as Character;
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
            Character character = EntityManager.Instance.GetEntity(healedEntityID) as Character;
            if (character == null || !character.IsAlive)
            {
                return;
            }

            character.CurrentHP += heal;

            LOP.Game.Current.GameEventManager.SendToNear(new EntityHeal(healingEntityID, healedEntityID, heal, character.CurrentHP), character.Position);
        }


        public void EntityGetMoney(int entityID, int money, Vector3 position)
        {
            MonoEntityBase entity = EntityManager.Instance.GetEntity(entityID) as MonoEntityBase;

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
            MonoEntityBase entity = EntityManager.Instance.GetEntity(entityID) as MonoEntityBase;

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

            MonoEntityBase attacker = EntityManager.Instance.GetEntity(nAttackerID) as MonoEntityBase;
            MonoEntityBase attacked = EntityManager.Instance.GetEntity(nAttackedID) as MonoEntityBase;

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
            Character entity = EntityManager.Instance.GetEntity(nAttackedID) as Character;
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
            GameItem gameItem = EntityManager.Instance.GetEntity(nAttackedID) as GameItem;
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
            Character attacker = EntityManager.Instance.GetEntity(nAttackerID) as Character;
            Character died = EntityManager.Instance.GetEntity(nDiedID) as Character;

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
            MonoEntityBase entity = EntityManager.Instance.GetEntity(nEntityID) as MonoEntityBase;

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
