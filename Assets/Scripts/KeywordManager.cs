using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeywordManager : MonoBehaviour
{
    public static KeywordManager Instance;

    public TextMeshProUGUI keywordListText;

    private List<KeywordData> keywords = new List<KeywordData>();

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

    void Start()
    {
        StartCoroutine(LoadKeywordsFromServer());
    }

    // 서버에서 키워드 불러오기
    IEnumerator LoadKeywordsFromServer()
    {
        yield return StartCoroutine(ApiManager.Instance.Get(
            "/keywords",
            (response) =>
            {
                var result = JsonUtility.FromJson<KeywordListResponse>(response);
                keywords.Clear();

                foreach (var item in result.data)
                {
                    KeywordData data = new KeywordData(item.name, ConvertCategory(item.category));
                    data.serverId = item.id;
                    data.description = item.description;

                    // 기본값 설정
                    data.basePrice = 10;
                    data.popularity = 10;
                    data.freshness = 10;
                    data.stability = 10;

                    keywords.Add(data);
                }

                UpdateKeywordUI();
                Debug.Log($"서버에서 키워드 {keywords.Count}개 로드 완료!");
            },
            (error) =>
            {
                Debug.LogError("키워드 로드 실패: " + error);
            }
        ));
    }

    KeywordType ConvertCategory(string category)
    {
        switch (category)
        {
            case "BASE": return KeywordType.Base;
            case "STYLE": return KeywordType.Style;
            case "CONCEPT": return KeywordType.Concept;
            default: return KeywordType.Base;
        }
    }

    // 탐험에서 키워드 획득 시 호출
    public void AddKeyword(int serverId, string name, KeywordType type)
    {
        // 이미 있는 키워드면 수량만 증가
        KeywordData existing = keywords.Find(k => k.serverId == serverId);
        if (existing != null)
        {
            Debug.Log("키워드 추가 획득: " + name);
            return;
        }

        KeywordData data = new KeywordData(name, type);
        data.serverId = serverId;
        keywords.Add(data);
        UpdateKeywordUI();
        Debug.Log("키워드 획득: " + name);
    }

    private void UpdateKeywordUI()
    {
        if (keywordListText == null) return;

        if (keywords.Count == 0)
        {
            keywordListText.text = "없음";
            return;
        }

        string baseText = "<b>[Base]</b>\n";
        string styleText = "<b>[Style]</b>\n";
        string conceptText = "<b>[Concept]</b>\n";

        bool hasBase = false, hasStyle = false, hasConcept = false;

        foreach (var data in keywords)
        {
            if (data.keywordType == KeywordType.Base)
            { baseText += "- " + data.keywordName + "\n"; hasBase = true; }
            else if (data.keywordType == KeywordType.Style)
            { styleText += "- " + data.keywordName + "\n"; hasStyle = true; }
            else if (data.keywordType == KeywordType.Concept)
            { conceptText += "- " + data.keywordName + "\n"; hasConcept = true; }
        }

        if (!hasBase) baseText += "없음\n";
        if (!hasStyle) styleText += "없음\n";
        if (!hasConcept) conceptText += "없음\n";

        keywordListText.text = baseText + "\n" + styleText + "\n" + conceptText;
    }

    public List<KeywordData> GetKeywords()
    {
        return keywords;
    }

    public void ClearKeywords()
    {
        keywords.Clear();
        UpdateKeywordUI();
    }

    // 응답 데이터 클래스
    [System.Serializable]
    private class KeywordItem
    {
        public int id;
        public string name;
        public string category;
        public string rarity;
        public string description;
    }

    [System.Serializable]
    private class KeywordListResponse
    {
        public string status;
        public List<KeywordItem> data;
    }
}