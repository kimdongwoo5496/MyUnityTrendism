using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PromotionPanelUI : MonoBehaviour
{
    [Header("텍스트")]
    public TextMeshProUGUI selectedItemText;
    public TextMeshProUGUI resultText;

    [Header("드롭다운")]
    public TMP_Dropdown backgroundDropdown;
    public TMP_Dropdown filterDropdown;
    public TMP_Dropdown hashtagDropdown1;
    public TMP_Dropdown hashtagDropdown2;
    public TMP_Dropdown hashtagDropdown3;

    [Header("버튼")]
    public Button promoteButton;
}