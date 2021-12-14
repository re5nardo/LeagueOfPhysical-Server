using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Entity;

public class JumpWang : SubGameBase
{
    private const float TIME_LIMIT = 60 * 2 * 60;

    private void Start()
    {
        SceneMessageBroker.AddSubscriber<GameMessage.EntityRegister>(OnEntityRegister);
    }

    private void OnDestroy()
    {
        SceneMessageBroker.RemoveSubscriber<GameMessage.EntityRegister>(OnEntityRegister);
    }

    private void OnEntityRegister(GameMessage.EntityRegister message)
    {
        var entity = Entities.Get<LOPMonoEntityBase>(message.entityId);

        entity.Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        if (entity.EntityRole == EntityRole.Player)
        {
            var spawnPoint = LOP.Game.Current.MapData.spawnPoints[Random.Range(0, LOP.Game.Current.MapData.spawnPoints.Length)];
            entity.Position = spawnPoint.position;
            entity.Rotation = spawnPoint.rotation;
        }
    }

    protected override IEnumerator OnInitialize()
    {
        foreach (var entity in Entities.GetAll<LOPMonoEntityBase>())
        {
            entity.Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }

        Physics.gravity *= LOP.Game.Current.MapData.mapEnvironment.GravityFactor;

        yield break;
    }

    protected override IEnumerator OnFinalize()
    {
        yield break;
    }

    protected override void OnGameStart()
    {
        var playerCharacters = Entities.GetAll<Character>(EntityRole.Player);

        foreach (var playerCharacter in playerCharacters)
        {
            var spawnPoint = LOP.Game.Current.MapData.spawnPoints[Random.Range(0, LOP.Game.Current.MapData.spawnPoints.Length)];
            playerCharacter.Position = spawnPoint.position;
            playerCharacter.Rotation = spawnPoint.rotation;
        }
    }

    protected override void OnGameEnd()
    {
    }

    protected override void OnTick(int tick)
    {
        if (ElapsedTime >= TIME_LIMIT)
        {
            EndGame();
        }
    }

    protected override void OnEarlyTickEnd(int tick)
    {
    }
}
