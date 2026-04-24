using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class RumorLogUI : MonoBehaviour
{
    [Header("소문 목록 UI")]
    public Transform contentParent;
    public GameObject rumorEntryTemplate;

    [Header("트렌드 빈도 UI")]
    public TMP_Text trendCountText;

    private readonly List<GameObject> spawnedEntries = new List<GameObject>();

    public void RefreshRumorUI()
    {
        RefreshRumorList();
        RefreshTrendCounts();
    }

    private void RefreshRumorList()
    {
        ClearEntries();

        if (RumorManager.Instance == null)
        {
            Debug.LogWarning("RumorManager가 없습니다.");
            return;
        }

        List<RumorData> rumors = RumorManager.Instance.GetAllRumors();

        for (int i = 0; i < rumors.Count; i++)
        {
            GameObject entry = Instantiate(rumorEntryTemplate, contentParent);
            entry.SetActive(true);

            TMP_Text text = entry.GetComponent<TMP_Text>();

            if (text != null)
            {
                string rareMark = rumors[i].isRareHint ? "[희귀] " : "";
                text.text = $"{rareMark}{rumors[i].rumorText}";
            }

            spawnedEntries.Add(entry);
        }
    }

    private void RefreshTrendCounts()
    {
        if (trendCountText == null) return;

        if (RumorManager.Instance == null)
        {
            trendCountText.text = "소문 데이터 없음";
            return;
        }

        Dictionary<string, int> counts = RumorManager.Instance.keywordCounts;

        if (counts == null || counts.Count == 0)
        {
            trendCountText.text = "아직 수집된 소문 없음";
            return;
        }

        StringBuilder sb = new StringBuilder();

        foreach (var pair in counts)
        {
            sb.AppendLine($"{pair.Key} : {pair.Value}회");
        }

        trendCountText.text = sb.ToString();
    }

    private void ClearEntries()
    {
        for (int i = 0; i < spawnedEntries.Count; i++)
        {
            Destroy(spawnedEntries[i]);
        }

        spawnedEntries.Clear();
    }
}