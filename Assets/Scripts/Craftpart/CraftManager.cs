using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    public static CraftManager Instance;

    [Header("선택된 키워드")]
    public List<KeywordData> selectedKeywords = new List<KeywordData>();

    [Header("슬롯 텍스트")]
    public TextMeshProUGUI slot1Text;
    public TextMeshProUGUI slot2Text;
    public TextMeshProUGUI slot3Text;

    [Header("미리보기 텍스트")]
    public TextMeshProUGUI previewNameText;
    public TextMeshProUGUI previewPriceText;
    public TextMeshProUGUI previewTargetText;
    public TextMeshProUGUI previewTrendText;

    [Header("결과 팝업")]
    public ResultPopupUI resultPopupUI;

    [Header("최근 제작 결과")]
    public CraftedItemResult lastCraftResult;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateSelectedSlots();
        UpdatePreview();
    }

    public void AddSelectedKeyword(KeywordData keyword)
    {
        if (keyword == null) return;

        if (selectedKeywords.Contains(keyword))
        {
            Debug.Log("이미 선택된 키워드입니다.");
            return;
        }

        if (selectedKeywords.Count >= 3)
        {
            Debug.Log("최대 3개까지만 선택 가능합니다.");
            return;
        }

        selectedKeywords.Add(keyword);
        UpdateSelectedSlots();
        UpdatePreview();
    }

    public void RemoveLastKeyword()
    {
        if (selectedKeywords.Count == 0) return;

        selectedKeywords.RemoveAt(selectedKeywords.Count - 1);
        UpdateSelectedSlots();
        UpdatePreview();
    }

    public void ClearSelectedKeywords()
    {
        selectedKeywords.Clear();
        UpdateSelectedSlots();
        UpdatePreview();
    }

    void UpdateSelectedSlots()
    {
        slot1Text.text = selectedKeywords.Count > 0 ? selectedKeywords[0].keywordName : "키워드 선택";
        slot2Text.text = selectedKeywords.Count > 1 ? selectedKeywords[1].keywordName : "키워드 선택";
        slot3Text.text = selectedKeywords.Count > 2 ? selectedKeywords[2].keywordName : "키워드 선택";
    }

    void UpdatePreview()
    {
        if (selectedKeywords.Count == 0)
        {
            previewNameText.text = "결과물 없음";
            previewPriceText.text = "예상 가격 : -";
            previewTargetText.text = "타겟 고객층 : -";
            previewTrendText.text = "유행성 : -";
            return;
        }

        previewNameText.text = GetPreviewName();
        previewPriceText.text = "예상 가격 : " + GetEstimatedPrice() + " G";
        previewTargetText.text = "타겟 고객층 : " + GetTargetAudience();
        previewTrendText.text = "유행성 : " + GetTrendLevel();
    }

    string GetPreviewName()
    {
        if (selectedKeywords.Count == 1)
            return selectedKeywords[0].keywordName + " 상품";

        string result = "";

        for (int i = 0; i < selectedKeywords.Count; i++)
        {
            result += selectedKeywords[i].keywordName;

            if (i < selectedKeywords.Count - 1)
                result += " + ";
        }

        result += " 조합품";
        return result;
    }

    int GetEstimatedPrice()
    {
        int total = 0;

        for (int i = 0; i < selectedKeywords.Count; i++)
        {
            total += selectedKeywords[i].basePrice;
        }

        if (selectedKeywords.Count == 3)
            total += 20;

        return total;
    }

    string GetTargetAudience()
    {
        for (int i = 0; i < selectedKeywords.Count; i++)
        {
            if (!string.IsNullOrEmpty(selectedKeywords[i].raceTag))
            {
                return selectedKeywords[i].raceTag + " 고객층";
            }
        }

        return "대중형";
    }

    string GetTrendLevel()
    {
        int score = 0;

        for (int i = 0; i < selectedKeywords.Count; i++)
        {
            score += selectedKeywords[i].popularity;
            score += selectedKeywords[i].freshness;
        }

        if (score >= 100) return "매우 높음";
        if (score >= 70) return "높음";
        if (score >= 40) return "보통";
        return "낮음";
    }

    public CraftedItemResult CraftItem()
{
    CraftedItemResult result = new CraftedItemResult();

    int craftScore = EvaluateCraftScore();

    result.itemName = GetPreviewName();
    result.finalPrice = GetEstimatedPrice() + craftScore;
    result.grade = GetGradeByScore(craftScore);
    result.targetAudience = GetTargetAudience();
    result.trendLevel = GetTrendLevel();
    result.craftScore = craftScore;

    for (int i = 0; i < selectedKeywords.Count; i++)
    {
        result.usedKeywordNames.Add(selectedKeywords[i].keywordName);
    }

    return result;
}

    int EvaluateCraftScore()
    {
        int score = 0;

        bool hasBase = false;
        bool hasStyle = false;
        bool hasConcept = false;

        for (int i = 0; i < selectedKeywords.Count; i++)
        {
            KeywordData keyword = selectedKeywords[i];

            score += keyword.basePrice / 2;
            score += keyword.popularity / 2;
            score += keyword.freshness / 2;
            score += keyword.stability / 2;

            if (keyword.keywordType == KeywordType.Base) hasBase = true;
            if (keyword.keywordType == KeywordType.Style) hasStyle = true;
            if (keyword.keywordType == KeywordType.Concept) hasConcept = true;
        }

        if (hasBase && hasStyle && hasConcept)
            score += 20;

        if (selectedKeywords.Count == 3)
            score += 10;

        return score;
    }

    string GetGradeByScore(int score)
    {
        if (score >= 90) return "S";
        if (score >= 70) return "A";
        if (score >= 45) return "B";
        return "C";
    }

    public void OnClickCraft()
    {
        if (selectedKeywords.Count == 0)
            {
            Debug.Log("선택된 키워드가 없습니다.");
            return;
            }

        lastCraftResult = CraftItem();

     if (CraftedItemManager.Instance != null)
     {
         CraftedItemManager.Instance.AddCraftedItem(lastCraftResult);
     }  

     if (RecipeBookManager.Instance != null)
     {
         RecipeBookManager.Instance.SaveRecipe(selectedKeywords, lastCraftResult);
     }  

        if (resultPopupUI != null)
        {
            resultPopupUI.Show(lastCraftResult);
        }

        if (craftedItemListUI != null)
        {
            craftedItemListUI.RefreshList();
        }

        Debug.Log("제작 완료: " + lastCraftResult.itemName + " / 등급: " + lastCraftResult.grade);

        ClearSelectedKeywords();
    }

    public CraftedItemListUI craftedItemListUI;
}