using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SellManager : MonoBehaviour
{
    public static SellManager Instance;

    [Header("판매 가능 아이템 목록")]
    public List<SellableItemData> sellableItems = new List<SellableItemData>();

    [Header("현재 선택 아이템")]
    public SellableItemData selectedItem;

    [Header("상단 UI")]
    public TextMeshProUGUI goldText;

    [Header("왼쪽 목록 UI")]
    public SellItemListUI sellItemListUI;

    [Header("중앙 UI")]
    public TextMeshProUGUI selectedItemTitleText;
    public TextMeshProUGUI itemInfoText;
    public TextMeshProUGUI trendValueText;
    public TextMeshProUGUI currentPriceText;

    [Header("오른쪽 로그 UI")]
    public TextMeshProUGUI logText;

    [Header("하단 정산 UI")]
    public TextMeshProUGUI summaryText;

    [Header("유행 슬라이더")]
    public Slider trendSlider;

    [Header("플레이어 골드")]
    public int currentGold = 0;

    [Header("정산 데이터")]
    public int totalSales = 0;
    public int totalDiscountSales = 0;
    public int totalSoldCount = 0;

    [Header("판매 로그 데이터")]
    public List<string> sellLogs = new List<string>();

    [Header("이미 반영한 제작 결과 개수")]
    public int processedCraftedItemCount = 0;

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
            return;
        }
    }

    private void Start()
    {
        // 처음 판매씬 진입 시에도 동작하게
        OnEnterSellScene();
    }

    public void OnEnterSellScene()
    {
        SyncNewCraftedItems();

        if (selectedItem == null && sellableItems.Count > 0)
            selectedItem = sellableItems[0];

        RefreshUI();
    }

    void SyncNewCraftedItems()
    {
        if (CraftedItemManager.Instance == null)
        {
            Debug.LogWarning("CraftedItemManager가 없습니다.");
            return;
        }

        List<CraftedItemResult> craftedItems = CraftedItemManager.Instance.GetCraftedItems();

        if (processedCraftedItemCount < 0)
            processedCraftedItemCount = 0;

        for (int i = processedCraftedItemCount; i < craftedItems.Count; i++)
        {
            CraftedItemResult crafted = craftedItems[i];
            SellableItemData existingItem = FindSameSellItem(crafted);

            if (existingItem != null)
            {
                existingItem.stock += 1;
            }
            else
            {
                SellableItemData newItem = new SellableItemData(crafted);
                newItem.stock = 1;
                newItem.trendValue = GetStartTrendValue(crafted);
                newItem.lifeTurns = 6;

                sellableItems.Add(newItem);
            }
        }

        processedCraftedItemCount = craftedItems.Count;
    }

    SellableItemData FindSameSellItem(CraftedItemResult crafted)
    {
        for (int i = 0; i < sellableItems.Count; i++)
        {
            CraftedItemResult target = sellableItems[i].craftedItem;

            bool sameName = target.itemName == crafted.itemName;
            bool sameGrade = target.grade == crafted.grade;
            bool samePrice = target.finalPrice == crafted.finalPrice;

            if (sameName && sameGrade && samePrice)
            {
                return sellableItems[i];
            }
        }

        return null;
    }

    float GetStartTrendValue(CraftedItemResult item)
    {
        float value = 40f;

        if (item.grade == "S") value += 30f;
        else if (item.grade == "A") value += 20f;
        else if (item.grade == "B") value += 10f;

        if (item.trendLevel == "매우 높음") value += 20f;
        else if (item.trendLevel == "높음") value += 10f;
        else if (item.trendLevel == "보통") value += 5f;

        return Mathf.Clamp(value, 0f, 100f);
    }

    int CalculateCurrentPrice(SellableItemData item)
    {
        if (item == null || item.craftedItem == null)
            return 0;

        float multiplier = 0.5f + (item.trendValue / 100f);
        return Mathf.RoundToInt(item.craftedItem.finalPrice * multiplier);
    }

    int CalculateDiscountPrice(SellableItemData item)
    {
        int normalPrice = CalculateCurrentPrice(item);
        return Mathf.Max(1, Mathf.RoundToInt(normalPrice * 0.7f));
    }

    public void SelectItem(SellableItemData item)
    {
        selectedItem = item;
        RefreshUI();
    }

    public void RefreshUI()
    {
        RefreshTopUI();
        RefreshCenterUI();
        RefreshSummaryUI();
        RefreshLogUI();

        if (sellItemListUI != null)
        {
            sellItemListUI.RefreshList(sellableItems);
        }
    }

    void RefreshTopUI()
    {
        if (goldText != null)
        {
            goldText.text = "보유 골드 : " + currentGold + " G";
        }
    }

    void RefreshCenterUI()
    {
        if (selectedItem == null)
        {
            if (selectedItemTitleText != null)
                selectedItemTitleText.text = "선택된 아이템 없음";

            if (itemInfoText != null)
                itemInfoText.text =
                    "등급 : -\n" +
                    "기본 가격 : -\n" +
                    "타겟 고객층 : -\n" +
                    "유행성 : -\n" +
                    "재고 : -\n" +
                    "남은 수명 : -";

            if (trendValueText != null)
                trendValueText.text = "유행 지수 : -";

            if (currentPriceText != null)
                currentPriceText.text = "현재 판매가 : -";

            if (trendSlider != null)
                trendSlider.value = 0f;

            return;
        }

        CraftedItemResult crafted = selectedItem.craftedItem;

        if (selectedItemTitleText != null)
            selectedItemTitleText.text = crafted.itemName;

        if (itemInfoText != null)
        {
            itemInfoText.text =
                "등급 : " + crafted.grade + "\n" +
                "기본 가격 : " + crafted.finalPrice + " G\n" +
                "타겟 고객층 : " + crafted.targetAudience + "\n" +
                "유행성 : " + crafted.trendLevel + "\n" +
                "재고 : " + selectedItem.stock + "\n" +
                "남은 수명 : " + selectedItem.lifeTurns;
        }

        if (trendValueText != null)
            trendValueText.text = "유행 지수 : " + Mathf.RoundToInt(selectedItem.trendValue);

        if (currentPriceText != null)
            currentPriceText.text = "현재 판매가 : " + CalculateCurrentPrice(selectedItem) + " G";

        if (trendSlider != null)
            trendSlider.value = selectedItem.trendValue / 100f;
    }

    public void OnClickSellNormal()
    {
        if (selectedItem == null)
        {
            AddLog("선택된 아이템이 없습니다.");
            return;
        }

        if (selectedItem.stock <= 0)
        {
            AddLog("재고가 없습니다.");
            return;
        }

        int sellPrice = CalculateCurrentPrice(selectedItem);

        selectedItem.stock -= 1;
        currentGold += sellPrice;
        totalSales += sellPrice;
        totalSoldCount += 1;

        AddLog("[정가 판매] " + selectedItem.craftedItem.itemName + " / +" + sellPrice + " G");

        RemoveItemIfEmpty(selectedItem);
        RefreshUI();
    }

    public void OnClickSellDiscount()
    {
        if (selectedItem == null)
        {
            AddLog("선택된 아이템이 없습니다.");
            return;
        }

        if (selectedItem.stock <= 0)
        {
            AddLog("재고가 없습니다.");
            return;
        }

        int sellPrice = CalculateDiscountPrice(selectedItem);

        selectedItem.stock -= 1;
        currentGold += sellPrice;
        totalSales += sellPrice;
        totalDiscountSales += sellPrice;
        totalSoldCount += 1;

        AddLog("[할인 판매] " + selectedItem.craftedItem.itemName + " / +" + sellPrice + " G");

        RemoveItemIfEmpty(selectedItem);
        RefreshUI();
    }

    void RemoveItemIfEmpty(SellableItemData item)
    {
        if (item.stock > 0)
            return;

        sellableItems.Remove(item);
        AddLog(item.craftedItem.itemName + " 재고 소진");

        if (selectedItem == item)
        {
            if (sellableItems.Count > 0)
                selectedItem = sellableItems[0];
            else
                selectedItem = null;
        }
    }

    public void OnClickNextTick()
    {
        for (int i = 0; i < sellableItems.Count; i++)
        {
            UpdateTrend(sellableItems[i]);
        }

        AddLog("시간 경과: 유행 지수가 변동했습니다.");
        RefreshUI();
    }

    void UpdateTrend(SellableItemData item)
    {
        item.lifeTurns -= 1;

        float change = Random.Range(-15f, 16f);

        if (item.craftedItem.grade == "S") change += 8f;
        else if (item.craftedItem.grade == "A") change += 4f;

        if (item.lifeTurns <= 2)
            change -= 20f;

        item.trendValue = Mathf.Clamp(item.trendValue + change, 0f, 100f);
    }

    void AddLog(string message)
    {
        sellLogs.Insert(0, "[기록] " + message);
        RefreshLogUI();
    }

    void RefreshLogUI()
    {
        if (logText == null) return;

        if (sellLogs.Count == 0)
        {
            logText.text = "판매 로그가 여기에 표시됩니다.";
            return;
        }

        string text = "";

        for (int i = 0; i < sellLogs.Count; i++)
        {
            text += sellLogs[i] + "\n\n";
        }

        logText.text = text;
    }

    void RefreshSummaryUI()
    {
        if (summaryText != null)
        {
            summaryText.text =
                "총 판매 수익 : " + totalSales + " G" +
                "   |   할인 판매 수익 : " + totalDiscountSales + " G" +
                "   |   판매 개수 : " + totalSoldCount;
        }
    }

    public void GoBackToCraftScene()
    {
        SceneManager.LoadScene("CraftScene");
    }
}