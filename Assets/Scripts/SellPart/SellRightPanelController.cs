using UnityEngine;
using UnityEngine.UI;

public class SellRightPanelController : MonoBehaviour
{
    public Button logTabButton;
    public Button promotionTabButton;
    public Button settlementTabButton;

    public GameObject logPanel;
    public GameObject promotionPanel;
    public GameObject settlementPanel;

    private void Start()
    {
        if (logTabButton != null)
        {
            logTabButton.onClick.RemoveAllListeners();
            logTabButton.onClick.AddListener(ShowLogPanel);
        }

        if (promotionTabButton != null)
        {
            promotionTabButton.onClick.RemoveAllListeners();
            promotionTabButton.onClick.AddListener(ShowPromotionPanel);
        }

        if (settlementTabButton != null)
        {
            settlementTabButton.onClick.RemoveAllListeners();
            settlementTabButton.onClick.AddListener(ShowSettlementPanel);
        }

        ShowLogPanel();
    }

    public void ShowLogPanel()
    {
        if (logPanel != null) logPanel.SetActive(true);
        if (promotionPanel != null) promotionPanel.SetActive(false);
        if (settlementPanel != null) settlementPanel.SetActive(false);
    }

    public void ShowPromotionPanel()
    {
        if (logPanel != null) logPanel.SetActive(false);
        if (promotionPanel != null) promotionPanel.SetActive(true);
        if (settlementPanel != null) settlementPanel.SetActive(false);
    }

    public void ShowSettlementPanel()
    {
        if (logPanel != null) logPanel.SetActive(false);
        if (promotionPanel != null) promotionPanel.SetActive(false);
        if (settlementPanel != null) settlementPanel.SetActive(true);
    }
}