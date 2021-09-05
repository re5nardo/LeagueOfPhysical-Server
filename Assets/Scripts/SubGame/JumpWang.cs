using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Entity;

public class JumpWang : SubGameBase
{
    private const float TIME_LIMIT = 60 * 2;

    private int spawnIndex = 0;

    private void Start()
    {
        GamePubSubService.AddSubscriber(GameMessageKey.EntityRegister, OnEntityRegister);
    }

    private void OnDestroy()
    {
        GamePubSubService.RemoveSubscriber(GameMessageKey.EntityRegister, OnEntityRegister);
    }

    private void OnEntityRegister(object[] param)
    {
        int entityId = (int)param[0];

        var entity = Entities.Get<MonoEntityBase>(entityId);

        entity.Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        if (entity.EntityRole == EntityRole.Player)
        {
            entity.Position = new Vector3(spawnIndex++ * 10, 0, 0);
        }
    }

    protected override IEnumerator OnInitialize()
    {
        yield return SceneManager.LoadSceneAsync(LOP.Game.Current.GameManager.mapName, LoadSceneMode.Additive);

        foreach (var entity in Entities.GetAll<MonoEntityBase>())
        {
            if (entity is MapObjectBase)
            {
                continue;
            }

            entity.Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }

        Physics.gravity *= SubGameEnvironment.GravityFactor;
    }
    
    protected override void OnGameStart()
    {
        var playerCharacters = Entities.GetAll<Character>(EntityRole.Player);

        foreach (var playerCharacter in playerCharacters)
        {
            playerCharacter.Position = new Vector3(spawnIndex++ * 10, 0, 0);
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
