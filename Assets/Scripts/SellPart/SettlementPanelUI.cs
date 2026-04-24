using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettlementPanelUI : MonoBehaviour
{
    [Header("텍스트")]
    public TextMeshProUGUI detailText;

    [Header("막대 Fill")]
    public RectTransform salesBarFill;
    public RectTransform materialBarFill;
    public RectTransform rentBarFill;
    public RectTransform promotionBarFill;
    public RectTransform manageBarFill;
    public RectTransform profitBarFill;

    [Header("막대 라벨")]
    public TextMeshProUGUI salesBarLabel;
    public TextMeshProUGUI materialBarLabel;
    public TextMeshProUGUI rentBarLabel;
    public TextMeshProUGUI promotionBarLabel;
    public TextMeshProUGUI manageBarLabel;
    public TextMeshProUGUI profitBarLabel;
}