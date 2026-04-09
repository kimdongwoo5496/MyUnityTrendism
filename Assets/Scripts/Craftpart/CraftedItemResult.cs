using System;
using System.Collections.Generic;

[Serializable]
public class CraftedItemResult
{
    public string itemName;
    public int finalPrice;
    public string grade;
    public string targetAudience;
    public string trendLevel;
    public int craftScore;

    public List<string> usedKeywordNames = new List<string>();
}