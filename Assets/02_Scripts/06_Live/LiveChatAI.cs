using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LiveChatAI : MonoBehaviour
{
    [Header("Ollama")]
    [SerializeField]
    private string _ollamaUrl =
        "http://localhost:11434/api/generate";

    [SerializeField]
    private string _model = "qwen3:1.7b";

    [Header("AI Chat")]
    [SerializeField]
    private int _minCharacters = 10;

    [SerializeField]
    private int _maxCharacters = 25;

    public void GenerateChat(
        string foodName,
        Action<string> onCompleted)
    {
        if (string.IsNullOrWhiteSpace(foodName))
        {
            onCompleted?.Invoke(null);
            return;
        }

        StartCoroutine(
            RequestOllama(
                foodName,
                onCompleted
            )
        );
    }

    private IEnumerator RequestOllama(
        string foodName,
        Action<string> onCompleted)
    {
        string prompt =
            "너는 한국 인터넷 먹방 방송의 시청자다.\n" +
            $"스트리머가 지금 {foodName}을(를) 먹었다.\n" +
            "시청자가 채팅창에 쓸 법한 짧은 반응을 한 줄만 작성해라.\n" +
            "규칙:\n" +
            "- 반드시 한국어로 작성\n" +
            $"- {_minCharacters}~{_maxCharacters}자 정도\n" +
            "- 인터넷 방송 채팅처럼 자연스럽게 작성\n" +
            $"- {foodName}에 대한 반응이어야 한다\n" +
            "- 스트리머에게 직접 말하는 것처럼 작성하지 않는다\n" +
            "- 시청자가 방송을 보며 순간적으로 남기는 반응처럼 작성한다\n" +
            "- 이모티콘이나 이모지를 사용하지 않는다\n" +
            "- 닉네임을 만들지 않는다\n" +
            "- 설명하지 않는다\n" +
            "- 생각 과정을 출력하지 않는다\n" +
            "- 채팅 한 줄만 출력한다";

        string json =
            "{"
            + $"\"model\":\"{EscapeJson(_model)}\","
            + $"\"prompt\":\"{EscapeJson(prompt)}\","
            + "\"stream\":false,"
            + "\"think\":false"
            + "}";

        byte[] body =
            Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request =
            new UnityWebRequest(
                _ollamaUrl,
                "POST"
            );

        request.uploadHandler =
            new UploadHandlerRaw(body);

        request.downloadHandler =
            new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json"
        );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(
                $"[LiveChatAI] Ollama 요청 실패: {request.error}"
            );

            Debug.LogError(
                request.downloadHandler.text
            );

            onCompleted?.Invoke(null);
            yield break;
        }

        string response =
            request.downloadHandler.text;

        Debug.Log(
            $"[LiveChatAI] Ollama 원본 응답: {response}"
        );

        OllamaResponse ollamaResponse =
            JsonUtility.FromJson<OllamaResponse>(
                response
            );

        if (ollamaResponse == null ||
            string.IsNullOrWhiteSpace(
                ollamaResponse.response))
        {
            Debug.LogWarning(
                "[LiveChatAI] Ollama 응답에서 채팅을 가져오지 못했습니다."
            );

            onCompleted?.Invoke(null);
            yield break;
        }

        string chat =
            CleanChat(
                ollamaResponse.response
            );

        if (string.IsNullOrWhiteSpace(chat))
        {
            Debug.LogWarning(
                "[LiveChatAI] AI 채팅 정리에 실패했습니다."
            );

            onCompleted?.Invoke(null);
            yield break;
        }

        Debug.Log(
            $"[LiveChatAI] AI 채팅: {chat}"
        );

        onCompleted?.Invoke(chat);
    }

    private string CleanChat(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        text = text.Trim();

        if (text.StartsWith("\"") &&
            text.EndsWith("\"") &&
            text.Length >= 2)
        {
            text =
                text.Substring(
                    1,
                    text.Length - 2
                ).Trim();
        }

        string[] lines =
            text.Split(
                new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries
            );

        if (lines.Length == 0)
            return null;

        text = lines[0].Trim();

        if (text.StartsWith("Thinking", StringComparison.OrdinalIgnoreCase))
            return null;

        if (text.StartsWith("생각"))
            return null;

        if (text.Length > _maxCharacters)
            return null;

        return text;
    }

    private string EscapeJson(string text)
    {
        return text
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r");
    }

    [Serializable]
    private class OllamaResponse
    {
        public string response;
    }
}