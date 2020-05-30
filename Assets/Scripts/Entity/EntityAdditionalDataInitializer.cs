using GameFramework;

public class EntityAdditionalDataInitializer : MonoSingleton<EntityAdditionalDataInitializer>
{
    public CharacterGrowthData Initialize(CharacterGrowthData data)
    {
        data.Initialize(1, 0);

        return data;
    }

    public EmotionExpressionData Initialize(EmotionExpressionData data, int nEntityID)
    {
        //  Get data from server db
        //  ...

        data.m_listEmotionExpressionID.Clear();
        data.m_listEmotionExpressionID.Add(0);
        data.m_listEmotionExpressionID.Add(1);
        data.m_listEmotionExpressionID.Add(2);
        data.m_listEmotionExpressionID.Add(3);

        return data;
    }

    public EntityInventory Initialize(EntityInventory data, int nEntityID)
    {
        //  Get data from server db
        //  ...

        data.m_nMoney = 0;

        return data;
    }
}
