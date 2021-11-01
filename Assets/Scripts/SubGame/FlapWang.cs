using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Entity;

public class FlapWang : SubGameBase
{
    private const float TIME_LIMIT = 60 * 2;

    private void Start()
    {
        SceneMessageBroker.AddSubscriber<GameMessage.EntityRegister>(OnEntityRegister);
    }

    private void OnDestroy()
    {
        SceneMessageBroker.RemoveSubscriber<GameMessage.EntityRegister>(OnEntityRegister);
    }

    //  이미 게임 시작할 때 (OnGameStart), 처리 되어있어야 함. (나중에 제거)
    private void OnEntityRegister(GameMessage.EntityRegister message)
    {
        var entity = Entities.Get<LOPMonoEntityBase>(message.entityId);

        entity.Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        if (entity.EntityRole == EntityRole.Player)
        {
            var spawnPoint = LOP.Game.Current.GameManager.MapData.spawnPoints[Random.Range(0, LOP.Game.Current.GameManager.MapData.spawnPoints.Length)];
            entity.Position = spawnPoint.position;
            entity.Rotation = spawnPoint.rotation;
        }
    }

    protected override IEnumerator OnInitialize()
    {
        yield return SceneManager.LoadSceneAsync(LOP.Game.Current.GameManager.MapData.sceneName, LoadSceneMode.Additive);

        foreach (var entity in Entities.GetAll<LOPMonoEntityBase>())
        {
            entity.Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }

        Physics.gravity *= LOP.Game.Current.GameManager.MapData.mapEnvironment.GravityFactor;
    }

    protected override IEnumerator OnFinalize()
    {
        yield return SceneManager.UnloadSceneAsync(LOP.Game.Current.GameManager.MapData.sceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
    }

    protected override void OnGameStart()
    {
        var playerCharacters = Entities.GetAll<Character>(EntityRole.Player);

        foreach (var playerCharacter in playerCharacters)
        {
            var spawnPoint = LOP.Game.Current.GameManager.MapData.spawnPoints[Random.Range(0, LOP.Game.Current.GameManager.MapData.spawnPoints.Length)];
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
