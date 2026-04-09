using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ApiTest : MonoBehaviour
{
    private const string BASE_URL = "https://unfocusedly-pleurocarpous-gina.ngrok-free.dev/";

    void Start()
    {
        StartCoroutine(GetKeywords());
    }

    IEnumerator GetKeywords()
    {
        string url = BASE_URL + "keywords";
        Debug.Log("요청 URL: " + url);

        using var req = UnityWebRequest.Get(url);
        req.SetRequestHeader("ngrok-skip-browser-warning", "true");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ 연동 성공!");
            Debug.Log(req.downloadHandler.text);
        }
        else
        {
            Debug.LogError("❌ 실패: " + req.error);
            Debug.LogError("응답 코드: " + req.responseCode);
        }
    }
}