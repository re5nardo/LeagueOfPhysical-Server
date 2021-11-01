using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Entity;

public class FallingGame : SubGameBase
{
    private const float TIME_LIMIT = 60 * 2;

    private int spawnIndex = 0;

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

        entity.Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        if (entity.EntityRole == EntityRole.Player)
        {
            entity.Position = new Vector3(spawnIndex++ * 10, 100, 0);
        }
    }

    protected override IEnumerator OnInitialize()
    {
        yield return SceneManager.LoadSceneAsync(LOP.Game.Current.GameManager.MapData.sceneName, LoadSceneMode.Additive);

        foreach (var entity in Entities.GetAll<LOPMonoEntityBase>())
        {
            if (entity.EntityRole != EntityRole.Player)
            {
                continue;
            }

            entity.Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
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
            playerCharacter.Position = new Vector3(spawnIndex++ * 10, 100, 0);
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
