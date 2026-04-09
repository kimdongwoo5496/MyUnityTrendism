using System.Collections.Generic;
using UnityEngine;

public class SellItemListUI : MonoBehaviour
{
    public Transform contentParent;
    public GameObject sellItemButtonPrefab;

    public void RefreshList(List<SellableItemData> sellableItems)
    {
        if (contentParent == null || sellItemButtonPrefab == null)
        {
            Debug.LogWarning("SellItemListUI 연결이 비어 있습니다.");
            return;
        }

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < sellableItems.Count; i++)
        {
            GameObject obj = Instantiate(sellItemButtonPrefab, contentParent);
            SellItemButtonUI ui = obj.GetComponent<SellItemButtonUI>();

            if (ui != null)
            {
                ui.Setup(sellableItems[i]);
            }
        }
    }
}