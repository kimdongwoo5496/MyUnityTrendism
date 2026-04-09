using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellItemButtonUI : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemInfoText;
    public Button button;
    public Image backgroundImage;

    private SellableItemData itemData;

    public void Setup(SellableItemData data)
    {
        itemData = data;

        if (itemNameText != null)
            itemNameText.text = data.craftedItem.itemName;

        if (itemInfoText != null)
            itemInfoText.text = "재고 " + data.stock +
                                " / " + data.craftedItem.grade + "등급" +
                                " / " + data.craftedItem.finalPrice + "G";

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickItem);
        }

        RefreshSelectionVisual();
    }

    public void RefreshSelectionVisual()
    {
        if (backgroundImage == null || SellManager.Instance == null)
            return;

        bool isSelected = (SellManager.Instance.selectedItem == itemData);

        if (isSelected)
            backgroundImage.color = new Color(0.45f, 0.65f, 0.95f, 1f);
        else
            backgroundImage.color = new Color(0.36f, 0.40f, 0.51f, 1f);
    }

    void OnClickItem()
    {
        if (SellManager.Instance != null)
        {
            SellManager.Instance.SelectItem(itemData);
        }
    }
}