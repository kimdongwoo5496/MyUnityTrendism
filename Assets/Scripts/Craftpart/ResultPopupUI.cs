using TMPro;
using UnityEngine;

public class ResultPopupUI : MonoBehaviour
{
    public GameObject popupRoot;

    public TextMeshProUGUI resultNameText;
    public TextMeshProUGUI resultGradeText;
    public TextMeshProUGUI resultPriceText;
    public TextMeshProUGUI resultTargetText;
    public TextMeshProUGUI resultTrendText;

    public void Show(CraftedItemResult result)
    {
        if (popupRoot != null)
            popupRoot.SetActive(true);

        if (resultNameText != null)
            resultNameText.text = "결과물 : " + result.itemName;

        if (resultGradeText != null)
            resultGradeText.text = "등급 : " + result.grade;

        if (resultPriceText != null)
            resultPriceText.text = "최종 가격 : " + result.finalPrice + " G";

        if (resultTargetText != null)
            resultTargetText.text = "타겟 고객층 : " + result.targetAudience;

        if (resultTrendText != null)
            resultTrendText.text = "유행성 : " + result.trendLevel;
    }

    public void Hide()
    {
        if (popupRoot != null)
            popupRoot.SetActive(false);
    }
}