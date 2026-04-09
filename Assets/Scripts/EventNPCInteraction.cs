using UnityEngine;

public class EventNPCInteraction : MonoBehaviour
{
    [TextArea]
    public string dialogueMessage = "도심에서 막 도착했어요. 요즘 상류층은 럭셔리한 분위기를 찾고 있답니다.";

    public string keywordToGive = "럭셔리";
    public KeywordType keywordType = KeywordType.Concept;

    public bool disappearAfterTalk = true;

    private bool playerInRange = false;
    private bool hasGivenKeyword = false;

    private DialogueManager dialogueManager;
    private UIManager uiManager;
    private KeywordManager keywordManager;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        uiManager = FindObjectOfType<UIManager>();
        keywordManager = FindObjectOfType<KeywordManager>();
    }

    void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (dialogueManager != null)
            {
                dialogueManager.OpenDialogue(dialogueMessage);
            }

            if (!hasGivenKeyword && keywordManager != null)
            {
                /*KeywordManager.Instance.AddKeyword(0, "키워드이름", KeywordType.Base);*/
                var master = keywordManager.GetMasterKeywordByName(keywordToGive);
                if (master != null)
                    keywordManager.AddKeyword(master.serverId, master.keywordName, master.keywordType);
                else
                    keywordManager.AddKeyword(0, keywordToGive, keywordType);
            hasGivenKeyword = true;
            }

            if (uiManager != null)
            {
                uiManager.HideInteractHint();
            }

            if (disappearAfterTalk)
            {
                Invoke(nameof(HideEventNPC), 1.0f);
            }
        }
    }

    void HideEventNPC()
    {
        transform.root.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (uiManager != null)
            {
                uiManager.ShowInteractHint("E키로 대화");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (uiManager != null)
            {
                uiManager.HideInteractHint();
            }
        }
    }
}