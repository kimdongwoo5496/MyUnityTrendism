using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [TextArea]
    public string dialogueMessage = "안녕, 요즘 마을에 이상한 유행이 돌고 있어.";

    public string keywordToGive = "차가운";
    public KeywordType keywordType = KeywordType.Style;

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
            if (dialogueManager.IsDialogueOpen())
            {
                dialogueManager.CloseDialogue();
            }
            else
            {
                dialogueManager.OpenDialogue(dialogueMessage);

                if (!hasGivenKeyword)
                {   
                    var master = keywordManager.GetMasterKeywordByName(keywordToGive);
                    if (master != null)
                        keywordManager.AddKeyword(master.serverId, master.keywordName, master.keywordType);
                    else
                        keywordManager.AddKeyword(0, keywordToGive, keywordType);
                    hasGivenKeyword = true;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            uiManager.ShowInteractHint("E키로 대화");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            uiManager.HideInteractHint();

            if (dialogueManager.IsDialogueOpen())
            {
                dialogueManager.CloseDialogue();
            }
        }
    }
}