using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeywordButtonUI : MonoBehaviour
{
    public TextMeshProUGUI keywordNameText;
    public TextMeshProUGUI typeText;
    public Button button;

    private KeywordData keywordData;

    public void Setup(KeywordData data)
    {
        keywordData = data;

        keywordNameText.text = data.keywordName;
        typeText.text = data.keywordType.ToString();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClickButton);
    }

    void OnClickButton()
    {
        if (CraftManager.Instance != null)
        {
            CraftManager.Instance.AddSelectedKeyword(keywordData);
        }
    }
}