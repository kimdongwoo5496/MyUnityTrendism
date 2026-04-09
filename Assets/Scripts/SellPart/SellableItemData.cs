using System;

[Serializable]
public class SellableItemData
{
    public CraftedItemResult craftedItem;

    public int stock = 1;
    public float trendValue = 50f;
    public int lifeTurns = 6;

    public SellableItemData(CraftedItemResult item)
    {
        craftedItem = item;
        stock = 1;
        trendValue = 50f;
        lifeTurns = 6;
    }
}