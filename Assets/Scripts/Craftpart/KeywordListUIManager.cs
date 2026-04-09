using System.Collections.Generic;
using UnityEngine;

public class KeywordListUIManager : MonoBehaviour
{
    public Transform contentParent;
    public GameObject keywordButtonPrefab;

    void Start()
    {
        LoadKeywordButtons();
    }

    public void LoadKeywordButtons()
    {
        KeywordManager keywordManager = FindObjectOfType<KeywordManager>();

        if (keywordManager == null)
        {
            Debug.LogWarning("KeywordManager를 찾을 수 없습니다.");
            return;
        }

        List<KeywordData> keywords = keywordManager.GetKeywords();

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < keywords.Count; i++)
        {
            GameObject obj = Instantiate(keywordButtonPrefab, contentParent);
            KeywordButtonUI ui = obj.GetComponent<KeywordButtonUI>();

            if (ui != null)
            {
                ui.Setup(keywords[i]);
            }
        }
    }
}