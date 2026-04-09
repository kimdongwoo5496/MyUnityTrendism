using UnityEngine;

public class ServerPingTest : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(ApiManager.Instance.Get(
            "/",
            (result) => Debug.Log("서버 연결 성공: " + result),
            (error) => Debug.LogError("서버 연결 실패: " + error)
        ));
    }
}