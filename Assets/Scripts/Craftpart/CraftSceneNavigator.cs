using UnityEngine;
using UnityEngine.SceneManagement;

public class CraftSceneNavigator : MonoBehaviour
{
    public void GoToSellScene()
    {
        SceneManager.LoadScene("SellScene");
    }
}