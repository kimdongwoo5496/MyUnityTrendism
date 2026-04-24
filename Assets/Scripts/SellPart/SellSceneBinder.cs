using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellSceneBinder : MonoBehaviour
{
    [Header("왼쪽 목록 UI")]
    public SellItemListUI sellItemListUI;

    [Header("상단 UI")]
    public TextMeshProUGUI goldText;

    [Header("중앙 UI")]
    public TextMeshProUGUI selectedItemTitleText;
    public TextMeshProUGUI itemInfoText;
    public TextMeshProUGUI trendValueText;
    public TextMeshProUGUI currentPriceText;
    public Slider trendSlider;

    [Header("오른쪽/하단 UI")]
    public TextMeshProUGUI logText;
    public TextMeshProUGUI summaryText;

    [Header("홍보 패널 UI")]
    public PromotionPanelUI promotionPanelUI;

    [Header("정산 패널 UI")]
    public SettlementPanelUI settlementPanelUI;

    [Header("버튼들")]
    public Button sellButton;
    public Button discountSellButton;
    public Button nextTickButton;
    public Button backToCraftButton;

    private void Start()
    {
        SellManager sellManager = SellManager.Instance;

        if (sellManager == null)
        {
            Debug.LogWarning("SellManager.Instance가 없습니다.");
            return;
        }

        sellManager.sellItemListUI = sellItemListUI;
        sellManager.goldText = goldText;
        sellManager.selectedItemTitleText = selectedItemTitleText;
        sellManager.itemInfoText = itemInfoText;
        sellManager.trendValueText = trendValueText;
        sellManager.currentPriceText = currentPriceText;
        sellManager.trendSlider = trendSlider;
        sellManager.logText = logText;
        sellManager.summaryText = summaryText;
        sellManager.promotionPanelUI = promotionPanelUI;
        sellManager.settlementPanelUI = settlementPanelUI;

        BindButtons(sellManager);
        sellManager.OnEnterSellScene();
    }

    void BindButtons(SellManager sellManager)
    {
        if (sellButton != null)
        {
            sellButton.onClick.RemoveAllListeners();
            sellButton.onClick.AddListener(sellManager.OnClickSellNormal);
        }

        if (discountSellButton != null)
        {
            discountSellButton.onClick.RemoveAllListeners();
            discountSellButton.onClick.AddListener(sellManager.OnClickSellDiscount);
        }

        if (nextTickButton != null)
        {
            nextTickButton.onClick.RemoveAllListeners();
            nextTickButton.onClick.AddListener(sellManager.OnClickNextTick);
        }

        if (backToCraftButton != null)
        {
            backToCraftButton.onClick.RemoveAllListeners();
            backToCraftButton.onClick.AddListener(sellManager.GoBackToCraftScene);
        }

        if (promotionPanelUI != null && promotionPanelUI.promoteButton != null)
        {
            promotionPanelUI.promoteButton.onClick.RemoveAllListeners();
            promotionPanelUI.promoteButton.onClick.AddListener(sellManager.OnClickPromote);
        }
    }
}