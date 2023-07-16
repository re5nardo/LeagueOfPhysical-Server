using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;
using System;
using GameFramework;

public partial class FlapWang : SubGameBase
{
    protected override IEnumerator OnInitialize()
    {
        Physics.gravity *= LOP.Game.Current.MapData.mapEnvironment.GravityFactor;

        LOP.Room.Instance.ExpectedPlayerList?.ForEach(expectedPlayer =>
        {
            var character = EntityHelper.CreatePlayerCharacter(expectedPlayer, Define.MasterData.CharacterID.GAREN, LOP.Application.UserId);

            LOP.Game.Current.gameIdMap.Add(expectedPlayer, character.EntityId);

            var spawnPoint = LOP.Game.Current.MapData.spawnPoints[UnityEngine.Random.Range(0, LOP.Game.Current.MapData.spawnPoints.Length)];
            character.Position = spawnPoint.position;
            character.Rotation = spawnPoint.rotation;
            character.Rigidbody.detectCollisions = false;
            character.Rigidbody.isKinematic = true;

            //  EntityAdditionalDatas
            CharacterGrowthData characterGrowthData = EntityAdditionalDataInitializer.Instance.Initialize(new CharacterGrowthData());
            character.AttachEntityComponent(characterGrowthData);

            EmotionExpressionData emotionExpressionData = EntityAdditionalDataInitializer.Instance.Initialize(new EmotionExpressionData(), character.EntityId);
            character.AttachEntityComponent(emotionExpressionData);

            EntityInventory entityInventory = EntityAdditionalDataInitializer.Instance.Initialize(new EntityInventory(), character.EntityId);
            character.AttachEntityComponent(entityInventory);

            character.AttachEntityComponent<NearEntityController>();

            character.AttachEntityComponent<PlayerView>();

            character.AttachEntityComponent<TransformSyncController>();
            if (character.ModelAnimator != null)
            {
                character.AttachEntityComponent<AnimatorSyncController>();
            }

            //  Entity Skill Info
            //  (should receive data from server db?)
            SkillController controller = character.GetComponent<SkillController>();
            foreach (int nSkillID in character.MasterData.SkillIDs)
            {
                controller.AddSkill(nSkillID);
            }

            using var disposer = PoolObjectDisposer<NetworkModel.Mirror.SC_PlayerEntity>.Get();
            var playerEntity = disposer.PoolObject;
            playerEntity.playerEntityId = character.EntityId;

            if (GameIdMap.TryGetConnectionIdByEntityId(character.EntityId, out var connectionId))
            {
                RoomNetwork.Instance.Send(playerEntity, connectionId);
            }
        });

        yield break;
    }

    protected override IEnumerator OnFinalize()
    {
        Physics.gravity /= LOP.Game.Current.MapData.mapEnvironment.GravityFactor;

        LOP.Game.Current.gameIdMap.Clear();

        yield break;
    }

    protected override void OnGameStart()
    {
        foreach (var userId in LOP.Game.Current.gameIdMap.GetAllUserIds())
        {
            if (LOP.Game.Current.gameIdMap.TryGetEntityId(userId, out var entityId))
            {
                var character = Entities.Get<Character>(entityId);
                character.OwnerId = userId;
                character.Rigidbody.detectCollisions = true;
                character.Rigidbody.isKinematic = false;
            }
            else
            {
                throw new Exception($"No entity of expectedPlayer. expectedPlayer: {userId}");
            }
        }
    }

    protected override void OnGameEnd()
    {
        foreach (var userId in LOP.Game.Current.gameIdMap.GetAllUserIds())
        {
            if (LOP.Game.Current.gameIdMap.TryGetEntityId(userId, out var entityId))
            {
                var character = Entities.Get<Character>(entityId);
                character.Rigidbody.detectCollisions = false;
                character.Rigidbody.isKinematic = true;
            }
            else
            {
                throw new Exception($"No entity of expectedPlayer. expectedPlayer: {userId}");
            }
        }
    }

    protected override void OnTick(int tick)
    {
        if (ElapsedTime >= TIME_WAIT_PLAYERS)
        {

        }

        if (ElapsedTime >= TIME_LIMIT)
        {
            EndGame();
        }
    }

    protected override void OnEarlyTickEnd(int tick)
    {
    }
}
