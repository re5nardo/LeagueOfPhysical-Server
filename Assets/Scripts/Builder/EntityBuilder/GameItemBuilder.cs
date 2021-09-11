using UnityEngine;
using Entity;

public class GameItemBuilder : EntityBuilder<GameItemBuilder, GameItem, GameItemCreationData>
{
    protected override GameItemCreationData entityCreationData { get; set; } = new GameItemCreationData();

    public GameItemBuilder SetMasterDataId(int masterDataId)
	{
        entityCreationData.masterDataId = masterDataId;
        return this;
	}

	public GameItemBuilder SetModelId(string modelId)
	{
        entityCreationData.modelId = modelId;
        return this;
	}

	public GameItemBuilder SetLifespan(float lifespan)
	{
        entityCreationData.lifespan = lifespan;
        return this;
	}

    public override GameItem Build()
	{
        GameObject goGameItem = new GameObject(string.Format("Entity_{0}", entityCreationData.entityId));
        GameItem gameItem = goGameItem.AddComponent<GameItem>();

        gameItem.Initialize(entityCreationData);

        EntityManager.Instance.RegisterEntity(gameItem);
		
		return gameItem;
	}
}
