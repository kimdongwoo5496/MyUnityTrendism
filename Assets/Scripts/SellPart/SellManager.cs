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

    [Header("고정 비용")]
    public int materialCost = 100;
    public int rentCost = 80;
    public int manageCost = 40;

    [Header("홍보 정산 데이터")]
    public int totalPromotionCost = 0;

    [Header("판매 로그 데이터")]
    public List<string> sellLogs = new List<string>();

    [Header("이미 반영한 제작 결과 개수")]
    public int processedCraftedItemCount = 0;

     [HideInInspector] 
     public PromotionPanelUI promotionPanelUI;

    [HideInInspector] public SettlementPanelUI settlementPanelUI; 

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

        float trendMultiplier = 0.7f + (item.trendValue / 100f); 
        float lifePenalty = 1f;

        if (item.lifeTurns <= 2)
            lifePenalty = 0.75f;
        else if (item.lifeTurns <= 4)
            lifePenalty = 0.9f;

        float finalValue = item.craftedItem.finalPrice * trendMultiplier * lifePenalty;
        return Mathf.Max(1, Mathf.RoundToInt(finalValue));
    }
    int CalculateDiscountPrice(SellableItemData item)
    {
        int normalPrice = CalculateCurrentPrice(item);
        return Mathf.Max(1, Mathf.RoundToInt(normalPrice * 0.7f));
    }

    float CalculatePromotionBoost(string background, string filter, string tag1, string tag2, string tag3)
    {
        float boost = 5f;

        // 배경 보너스
        if (background == "고급 천")
            boost += 2f;
       else if (background == "꽃 장식")
            boost += 3f;
        else if (background == "마법 조명")
            boost += 4f;

        // 필터 보너스
        if (filter == "빈티지")
            boost += 3f;
        else if (filter == "화사함")
            boost += 2f;
        else if (filter == "시크함")
            boost += 2.5f;

        // 해시태그 기본 보너스
        boost += 1.5f;

        // 중복 태그 패널티
        if (tag1 == tag2 || tag2 == tag3 || tag1 == tag3)
            boost -= 2f;

        // 선택된 아이템 이름과 태그/필터가 맞으면 추가 보너스
        if (selectedItem != null && selectedItem.craftedItem != null)
        {
            string itemName = selectedItem.craftedItem.itemName;

            if (itemName.Contains("감성"))
            {
                if (tag1 == "감성" || tag2 == "감성" || tag3 == "감성")
                    boost += 3f;
            }

            if (itemName.Contains("럭셔리"))
            {
                if (tag1 == "럭셔리" || tag2 == "럭셔리" || tag3 == "럭셔리")
                    boost += 3f;
            }

            if (itemName.Contains("빈티지"))
            {
                if (filter == "빈티지")
                    boost += 3f;
            }
        }

        // 같은 아이템을 반복 홍보할수록 효율 감소
        if (selectedItem != null)
        {
            boost -= selectedItem.promotionCount * 0.7f;
        }

        if (selectedItem != null)
        {
            if (selectedItem.lifeTurns <= 2)
                boost -= 3f;
            else if (selectedItem.lifeTurns <= 4)
                boost -= 1.5f;
        }

        return Mathf.Clamp(boost, 1f, 20f);
    }

    public void SelectItem(SellableItemData item)
    {
        selectedItem = item;
        RefreshUI();
        RefreshPromotionPanelUI();
    }

    public void RefreshUI()
    {
        RefreshTopUI();
        RefreshCenterUI();
        RefreshSummaryUI();
        RefreshLogUI();
        RefreshSettlementPanelUI();

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
                "재고 : " + selectedItem.stock + "\n" +
                "등급 : " + selectedItem.craftedItem.grade + "\n" +
                "남은 유행 턴 : " + selectedItem.lifeTurns + "\n" +
                "홍보 횟수 : " + selectedItem.promotionCount + "\n" +
                "마지막 홍보 효과 : +" + selectedItem.lastPromotionBoost.ToString("F1");

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

    public void OnClickPromote()
    {
        if (selectedItem == null)
        {
            if (promotionPanelUI != null && promotionPanelUI.resultText != null)
                promotionPanelUI.resultText.text = "홍보할 아이템을 먼저 선택하세요.";
            return;
        }

        if (promotionPanelUI == null)
            return;

        if (promotionPanelUI.backgroundDropdown == null ||
            promotionPanelUI.filterDropdown == null ||
            promotionPanelUI.hashtagDropdown1 == null ||
            promotionPanelUI.hashtagDropdown2 == null ||
            promotionPanelUI.hashtagDropdown3 == null)
        {
            return;
        }

        string background =
            promotionPanelUI.backgroundDropdown.options[promotionPanelUI.backgroundDropdown.value].text;

        string filter =
            promotionPanelUI.filterDropdown.options[promotionPanelUI.filterDropdown.value].text;

        string tag1 =
            promotionPanelUI.hashtagDropdown1.options[promotionPanelUI.hashtagDropdown1.value].text;

        string tag2 =
            promotionPanelUI.hashtagDropdown2.options[promotionPanelUI.hashtagDropdown2.value].text;

        string tag3 =
            promotionPanelUI.hashtagDropdown3.options[promotionPanelUI.hashtagDropdown3.value].text;

        int promotionCost = 20;

        if (currentGold < promotionCost)
        {
            if (promotionPanelUI.resultText != null)
                promotionPanelUI.resultText.text = "골드가 부족해서 홍보할 수 없습니다.";
            return;
        }

        float boost = CalculatePromotionBoost(background, filter, tag1, tag2, tag3);

        selectedItem.trendValue += boost;
        selectedItem.trendValue = Mathf.Clamp(selectedItem.trendValue, 0f, 100f);

        selectedItem.promotionCount += 1;
        selectedItem.lastPromotionBoost = boost;

        currentGold -= promotionCost;
        totalPromotionCost += promotionCost;

        if (promotionPanelUI.resultText != null)
        {
           promotionPanelUI.resultText.text =
                "홍보 성공!\n" +
                "배경 : " + background + "\n" +
                "필터 : " + filter + "\n" +
                "해시태그 : #" + tag1 + " #" + tag2 + " #" + tag3 + "\n" +
                "유행 지수 상승 : +" + boost.ToString("F1") + "\n" +
                "홍보비 : -" + promotionCost + " G\n" +
                "누적 홍보 횟수 : " + selectedItem.promotionCount;
        }

        AddLog("[홍보] " + selectedItem.craftedItem.itemName + " / 유행 지수 +" + boost.ToString("F1"));

        RefreshUI();
        RefreshPromotionPanelUI();
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
                "   |   판매 개수 : " + totalSoldCount +
                "   |   홍보비 : " + totalPromotionCost + " G";
        }
    }

    void RefreshPromotionPanelUI()
    {
        if (promotionPanelUI == null || promotionPanelUI.selectedItemText == null)
            return;

        if (selectedItem == null)
        {
            promotionPanelUI.selectedItemText.text = "선택 아이템: 없음";
            return;
        }

        promotionPanelUI.selectedItemText.text = "선택 아이템: " + selectedItem.craftedItem.itemName;
    }

    void RefreshSettlementPanelUI()
    {
        if (settlementPanelUI == null)
            return;

        int totalRevenue = totalSales + totalDiscountSales;
        int totalCost = materialCost + rentCost + manageCost + totalPromotionCost;
        int netProfit = totalRevenue - totalCost;

        if (settlementPanelUI.detailText != null)
        {
            settlementPanelUI.detailText.text =
                "총매출: " + totalRevenue + " G\n" +
                "재료비: " + materialCost + " G\n" +
                "임대료: " + rentCost + " G\n" +
                "홍보비: " + totalPromotionCost + " G\n" +
                "관리비: " + manageCost + " G\n" +
                "순이익: " + netProfit + " G";
        }

        int maxValue = Mathf.Max(totalRevenue, materialCost, rentCost, totalPromotionCost, manageCost, Mathf.Abs(netProfit), 1);

        SetBarWidth(settlementPanelUI.salesBarFill, totalRevenue, maxValue);
        SetBarWidth(settlementPanelUI.materialBarFill, materialCost, maxValue);
        SetBarWidth(settlementPanelUI.rentBarFill, rentCost, maxValue);
        SetBarWidth(settlementPanelUI.promotionBarFill, totalPromotionCost, maxValue);
        SetBarWidth(settlementPanelUI.manageBarFill, manageCost, maxValue);
        SetBarWidth(settlementPanelUI.profitBarFill, Mathf.Abs(netProfit), maxValue);

        if (settlementPanelUI.salesBarLabel != null)
            settlementPanelUI.salesBarLabel.text = "총매출  " + totalRevenue + " G";

        if (settlementPanelUI.materialBarLabel != null)
            settlementPanelUI.materialBarLabel.text = "재료비  " + materialCost + " G";

        if (settlementPanelUI.rentBarLabel != null)
            settlementPanelUI.rentBarLabel.text = "임대료  " + rentCost + " G";

        if (settlementPanelUI.promotionBarLabel != null)
            settlementPanelUI.promotionBarLabel.text = "홍보비  " + totalPromotionCost + " G";

        if (settlementPanelUI.manageBarLabel != null)
            settlementPanelUI.manageBarLabel.text = "관리비  " + manageCost + " G";

            if (settlementPanelUI.profitBarLabel != null)
            {
            if (netProfit >= 0)
                settlementPanelUI.profitBarLabel.text = "순이익  " + netProfit + " G";
            else
                settlementPanelUI.profitBarLabel.text = "순손실  " + netProfit + " G";
        }
        if (settlementPanelUI.profitBarFill != null)
        {
        Image profitImage = settlementPanelUI.profitBarFill.GetComponent<Image>();
        if (profitImage != null)
        {
            if (netProfit >= 0)
                profitImage.color = new Color(0.3f, 0.85f, 0.4f, 1f);
            else
                profitImage.color = new Color(0.9f, 0.3f, 0.3f, 1f);
            }
        }
    }

    void SetBarWidth(RectTransform barRect, int value, int maxValue)
    {
        if (barRect == null)
            return;

        float maxWidth = 260f;
        float ratio = (float)value / maxValue;
        ratio = Mathf.Clamp01(ratio);

        barRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth * ratio);
    }

    public void GoBackToCraftScene()
    {
        SceneManager.LoadScene("CraftScene");
    }
}