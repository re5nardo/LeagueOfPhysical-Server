using UnityEngine;
using Entity;
using EntityCommand;

public class GameItemBasicData : EntityBasicData
{
    private GameItem GameItem => Entity as GameItem;

    public int MasterDataId { get; private set; } = -1;

    private string modelId;
    public string ModelId
    {
        get => modelId;
        private set
        {
            modelId = value;
            Entity.SendCommandToViews(new ModelChanged(value));
        }
    }

    private int hp;
    public int HP
    {
        get => hp;
        set => hp = Mathf.Min(value, MaximumHP);
    }

    public int MaximumHP => GameItem.MasterData.HP;

    public float MovementSpeed => 0;

    public override void Initialize(EntityCreationData entityCreationData)
	{
		base.Initialize(entityCreationData);

        GameItemCreationData gameItemCreationData = entityCreationData as GameItemCreationData;

        MasterDataId = gameItemCreationData.masterDataId;
        ModelId = gameItemCreationData.modelId;

        HP = GameItem.MasterData.HP;
    }
}
