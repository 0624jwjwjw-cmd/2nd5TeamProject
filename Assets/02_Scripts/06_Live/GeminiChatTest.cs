using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GeminiChatTest : MonoBehaviour
{
    [SerializeField] private string _apiKey;

    private const string Model = "gemini-3.6-flash";

    private void Start()
    {
        GenerateChat();
    }

    public void GenerateChat()
    {
        StartCoroutine(RequestGemini());
    }

    private IEnumerator RequestGemini()
    {
        string url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:generateContent?key={_apiKey}";

        string prompt =
            "너는 인터넷 먹방 방송의 시청자다.\n" +
            "스트리머가 치즈버거를 한입에 먹었다.\n" +
            "한국 인터넷 방송에서 볼 법한 짧은 시청자 채팅을 5개 만들어줘.\n" +
            "각 채팅에는 닉네임을 붙여줘.";

        string json =
            "{\"contents\":[{\"parts\":[{\"text\":\"" +
            EscapeJson(prompt) +
            "\"}]}]}";

        byte[] body = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request = new UnityWebRequest(url, "POST");

        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Gemini 요청 실패: " + request.error);
            Debug.LogError(request.downloadHandler.text);
            yield break;
        }

        Debug.Log("Gemini 응답:");
        Debug.Log(request.downloadHandler.text);
    }

    private string EscapeJson(string text)
    {
        return text
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r");
    }
}