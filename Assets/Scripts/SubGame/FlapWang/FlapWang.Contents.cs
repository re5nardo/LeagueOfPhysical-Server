using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity;
using System;
using System.Linq;
using GameFramework;
using Random = UnityEngine.Random;

public partial class FlapWang
{
    private const float TIME_LIMIT = 60 * 2;
    private const float TIME_WAIT_PLAYERS = 5;

    private List<Character> playerCharacters = new List<Character>();

    private enum Phase
    {
        none = 0,
        ready = 1,
        inGame = 2,
        gameEnd = 3,
    }

    private Phase _phase;   //  ==> StateMachine
    private Phase phase
    {
        get => _phase;
        set
        {
            if (_phase != value)
            {
                _phase = value;
                OnPhaseChange();
            }
        }
    }

    private void Start()
    {
        SceneMessageBroker.AddSubscriber<GameMessage.EntityRegister>(OnEntityRegister);
        SceneMessageBroker.AddSubscriber<EntityMessage.ModelTriggerEnter>(OnModelTriggerEnter);
    }

    private void OnDestroy()
    {
        SceneMessageBroker.RemoveSubscriber<GameMessage.EntityRegister>(OnEntityRegister);
        SceneMessageBroker.RemoveSubscriber<EntityMessage.ModelTriggerEnter>(OnModelTriggerEnter);
    }

    private void OnEntityRegister(GameMessage.EntityRegister message)
    {
        if (phase == Phase.ready)
        {
            //  server에서 플레이어들 강제로 스폰 (Register) 시키고, 중간에 추가로 들어오는 일 없어야 함
        }

        var entity = Entities.Get<LOPMonoEntityBase>(message.entityId);

        entity.Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        if (entity.EntityRole == EntityRole.Player)
        {
            var spawnPoint = LOP.Game.Current.MapData.spawnPoints[Random.Range(0, LOP.Game.Current.MapData.spawnPoints.Length)];
            entity.Position = spawnPoint.position;
            entity.Rotation = spawnPoint.rotation;
        }
    }

    private void OnModelTriggerEnter(EntityMessage.ModelTriggerEnter message)
    {
        var entity = Entities.Get<LOPMonoEntityBase>(message.entityId);

        if (entity.EntityRole != EntityRole.Player
            || !(entity is Character character)
            || !character.IsAlive
            || character.HasStatusEffect(StatusEffect.Invincible)
            )
        {
            return;
        }

        Action<Behavior.BehaviorBase> onDieEnd = behavior =>
        {
            Resurrect(character);
        };

        Die(character, onDieEnd);
    }

    private void Die(Character character, Action<Behavior.BehaviorBase> onDieEnd)
    {
        character.Blackboard.Set("diePosition", character.Position);
        character.HP = 0;

        character.BehaviorController.StartBehavior(new BehaviorParam(Define.MasterData.BehaviorId.Die), onDieEnd);
    }

    private void Resurrect(Character character)
    {
        //  Transform
        character.Position = character.Blackboard.Get<Vector3>("diePosition", true);
        character.Rotation = new Vector3(0, 90, 0);
        character.Velocity = Vector3.zero;

        //  HP
        character.HP = character.MaximumHP;

        //  ContinuousMove
        character.BehaviorController.StartBehavior(new ContinuousMoveBehaviorParam(Define.MasterData.BehaviorId.ContinuousMove, Vector3.right));

        //  Invincible
        character.StateController.StartState(new BasicStateParam(Define.MasterData.StateId.Invincible, 2));
    }

    private void OnPhaseChange()
    {
        switch (phase)
        {
            case Phase.ready:
                LOP.Room.Instance.ExpectedPlayerList?.ForEach(expectedPlayer =>
                {
                    var character = LOP.Game.Current.CreatePlayerCharacter(expectedPlayer, Define.MasterData.CharacterID.GAREN);
                    var spawnPoint = LOP.Game.Current.MapData.spawnPoints[Random.Range(0, LOP.Game.Current.MapData.spawnPoints.Length)];
                    character.Position = spawnPoint.position;
                    character.Rotation = spawnPoint.rotation;
                    character.Rigidbody.detectCollisions = false;
                    character.Rigidbody.isKinematic = true;

                    playerCharacters.Add(character);
                });
                break;

            case Phase.inGame:
                foreach (var playerCharacter in playerCharacters)
                {
                    playerCharacter.Rigidbody.detectCollisions = true;
                    playerCharacter.Rigidbody.isKinematic = false;
                }
                break;

            case Phase.gameEnd:
                foreach (var playerCharacter in playerCharacters)
                {
                    playerCharacter.Rigidbody.detectCollisions = false;
                    playerCharacter.Rigidbody.isKinematic = true;
                }
                break;
        }
    }
}
