using UnityEngine;

public class ExploreNPC : MonoBehaviour
{
    [Header("NPC 정보")]
    public string npcName;
    public string zoneName;

    [Header("대사")]
    [TextArea(2, 4)]
    public string dialogueText;

    [Header("소문 정보")]
    [TextArea(2, 4)]
    public string rumorText;

    public string relatedKeyword;
    public bool isRareHint = false;

    private bool hasTalkedToday = false;

    public void Interact()
    {
        if (hasTalkedToday)
        {
            Debug.Log($"{npcName}는 오늘 이미 대화함");
            return;
        }

        Debug.Log($"{npcName} 대화: {dialogueText}");

        if (RumorManager.Instance != null)
        {
            RumorData data = new RumorData
            {
                rumorId = npcName + "_" + relatedKeyword,
                rumorText = rumorText,
                sourceNPC = npcName,
                zoneName = zoneName,
                relatedKeyword = relatedKeyword,
                isRareHint = isRareHint
            };

            RumorManager.Instance.AddRumor(data);
        }

        hasTalkedToday = true;
    }

    public void ResetDailyTalk()
    {
        hasTalkedToday = false;
    }
}