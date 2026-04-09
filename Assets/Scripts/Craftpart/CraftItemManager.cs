using System.Collections.Generic;
using UnityEngine;

public class CraftedItemManager : MonoBehaviour
{
    public static CraftedItemManager Instance;

    [Header("제작 완료된 아이템 목록")]
    public List<CraftedItemResult> craftedItems = new List<CraftedItemResult>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCraftedItem(CraftedItemResult item)
    {
        if (item == null) return;

        craftedItems.Add(item);
        Debug.Log("제작 결과 저장: " + item.itemName);
    }

    public List<CraftedItemResult> GetCraftedItems()
    {
        return craftedItems;
    }

    public void ClearCraftedItems()
    {
        craftedItems.Clear();
        Debug.Log("제작 결과 목록 초기화");
    }
}