
using System.Collections.Generic;
using UnityEngine;

public class RumorManager : MonoBehaviour
{
    public static RumorManager Instance;

    [Header("현재 수집한 소문 목록")]
    public List<RumorData> collectedRumors = new List<RumorData>();

    [Header("중복 카운트용")]
    public Dictionary<string, int> keywordCounts = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddRumor(RumorData newRumor)
{
    if (newRumor == null) return;

    collectedRumors.Add(newRumor);

    if (!string.IsNullOrEmpty(newRumor.relatedKeyword))
    {
        if (keywordCounts.ContainsKey(newRumor.relatedKeyword))
            keywordCounts[newRumor.relatedKeyword]++;
        else
            keywordCounts[newRumor.relatedKeyword] = 1;
    }

    Debug.Log("소문 추가: " + newRumor.rumorText);

    RumorLogUI rumorUI = FindObjectOfType<RumorLogUI>();

    if (rumorUI != null)
    {
        rumorUI.RefreshRumorUI();
    }
}

    public int GetKeywordCount(string keyword)
    {
        if (keywordCounts.ContainsKey(keyword))
            return keywordCounts[keyword];

        return 0;
    }

    public List<RumorData> GetAllRumors()
    {
        return collectedRumors;
    }
}