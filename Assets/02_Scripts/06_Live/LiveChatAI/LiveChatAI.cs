using System;
using System.Collections;
using System.IO;
using UnityEngine;
using LlamaCppUnity;

public class LiveChatAI : MonoBehaviour
{
    [Header("Qwen Model")]
    [SerializeField]
    private string _modelFileName =
        "qwen2.5-0.5b-instruct-q4_k_m.gguf";

    [Header("Generation")]
    [SerializeField]
    private int _maxTokens = 20;

    [SerializeField]
    private int _minCharacters = 2;

    [SerializeField]
    private int _maxCharacters = 20;

    private Llama _llama;

    private bool _isReady;
    private bool _isGenerating;


    private void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        _isReady = false;
        return;

#endif

        StartCoroutine(InitializeModel());
    }


    private IEnumerator InitializeModel()
    {
        string modelPath = Path.Combine(
            Application.streamingAssetsPath,
            "Models",
            _modelFileName
        );

        if (!File.Exists(modelPath))
        {
            _isReady = false;
            yield break;
        }

        yield return null;

        try
        {
            _llama = new Llama(
                modelPath,
                nGpuLayers: 0,
                nCtx: 2048,
                nBatch: 256,
                nThreads: null,
                verbose: false
            );

            _isReady = true;
        }
        catch (Exception)
        {
            _llama = null;
            _isReady = false;
        }
    }


    public void GenerateChat(
        string foodName,
        Action<string> onCompleted)
    {
        if (string.IsNullOrWhiteSpace(foodName))
        {
            onCompleted?.Invoke(null);
            return;
        }

        if (!_isReady || _llama == null)
        {
            onCompleted?.Invoke(null);
            return;
        }

        if (_isGenerating)
        {
            onCompleted?.Invoke(null);
            return;
        }

        StartCoroutine(
            GenerateRoutine(
                foodName,
                onCompleted
            )
        );
    }


    private IEnumerator GenerateRoutine(
        string foodName,
        Action<string> onCompleted)
    {
        _isGenerating = true;

        string result = null;
        Exception generationException = null;

        /*
         * Qwen 0.5B가 이해하기 쉽도록 단순한 프롬프트 사용
         */
        string prompt =
            "너는 먹방 방송 시청자야.\n" +
            "스트리머가 음식을 먹는 것을 보고 채팅을 쓴다.\n" +
            "짧은 채팅 한마디만 써.\n" +
            "자연스러운 한국 인터넷 방송 말투를 써.\n" +
            "음식 설명은 하지 마.\n" +
            "상황 설명은 하지 마.\n" +
            "닉네임은 쓰지 마.\n" +
            "질문하지 마.\n" +
            "한 줄만 써.\n" +
            "\n" +
            "예시:\n" +
            "와 개맛있겠다\n" +
            "한입만\n" +
            "와 진짜 잘먹네\n" +
            "저것도 먹네ㅋㅋ\n" +
            "맛있겠다\n" +
            "개맛있어보임\n" +
            "오늘 제대로 먹네\n" +
            "\n" +
            "음식: " + foodName + "\n" +
            "채팅:";

        yield return null;

        try
        {
            result = _llama.Run(
                prompt,
                maxTokens: (uint)_maxTokens
            );
        }
        catch (Exception)
        {
            generationException = new Exception();
        }

        if (generationException != null)
        {
            _isGenerating = false;

            onCompleted?.Invoke(null);
            yield break;
        }

        result = CleanChat(result);

        _isGenerating = false;

        if (string.IsNullOrWhiteSpace(result))
        {
            onCompleted?.Invoke(null);
            yield break;
        }

        onCompleted?.Invoke(result);
    }


    private string CleanChat(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        text = text.Trim();

        // Qwen 특수 토큰 제거
        text = text.Replace(
            "<|im_start|>",
            ""
        );

        text = text.Replace(
            "<|im_end|>",
            ""
        );

        text = text.Replace(
            "<|endoftext|>",
            ""
        );

        text = text.Trim();

        // 첫 번째 줄만 사용
        string[] lines =
            text.Split(
                new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries
            );

        if (lines.Length == 0)
            return null;

        text = lines[0].Trim();

        // AI가 채팅: 을 붙인 경우
        if (text.StartsWith("채팅:"))
        {
            text = text.Substring(3).Trim();
        }

        // AI가 답변: 을 붙인 경우
        if (text.StartsWith("답변:"))
        {
            text = text.Substring(3).Trim();
        }

        // AI가 Chat: 을 붙인 경우
        if (text.StartsWith("Chat:"))
        {
            text = text.Substring(5).Trim();
        }

        // AI가 닉네임을 만들어버린 경우
        int colonIndex = text.IndexOf(':');

        if (colonIndex > 0 && colonIndex <= 15)
        {
            text = text.Substring(
                colonIndex + 1
            ).Trim();
        }

        // 따옴표 제거
        if (text.StartsWith("\"") &&
            text.EndsWith("\"") &&
            text.Length >= 2)
        {
            text = text.Substring(
                1,
                text.Length - 2
            ).Trim();
        }

        // 불필요한 접두어 제거
        if (text.StartsWith("시청자:"))
        {
            text = text.Substring(4).Trim();
        }

        if (text.StartsWith("시청자"))
        {
            text = text.Substring(3).Trim();
        }

        if (string.IsNullOrWhiteSpace(text))
            return null;

        // 모델이 생각/설명문을 생성한 경우
        if (text.StartsWith(
                "Thinking",
                StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (text.StartsWith("생각"))
        {
            return null;
        }

        if (text.StartsWith("스트리머"))
        {
            return null;
        }

        if (text.StartsWith("시청자는"))
        {
            return null;
        }

        // 최대 길이 제한
        if (text.Length > _maxCharacters)
        {
            text = text.Substring(
                0,
                _maxCharacters
            ).Trim();
        }

        // 최소 길이 제한
        if (text.Length < _minCharacters)
            return null;

        return text;
    }


    private void OnDestroy()
    {
        _llama = null;
        _isReady = false;
        _isGenerating = false;
    }
}