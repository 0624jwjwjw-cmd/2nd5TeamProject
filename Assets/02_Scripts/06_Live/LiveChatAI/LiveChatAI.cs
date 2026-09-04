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
    private int _maxTokens = 32;

    [SerializeField]
    private int _minCharacters = 5;

    [SerializeField]
    private int _maxCharacters = 25;

    private Llama _llama;

    private bool _isReady;
    private bool _isGenerating;


    private void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        Debug.Log(
            "[LiveChatAI] Android에서는 AI를 사용하지 않습니다. 기본 채팅을 사용합니다."
        );

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

        Debug.Log(
            $"[LiveChatAI] Qwen 모델 경로: {modelPath}"
        );

        if (!File.Exists(modelPath))
        {
            Debug.LogError(
                $"[LiveChatAI] Qwen 모델을 찾을 수 없습니다: {modelPath}"
            );

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

            Debug.Log(
                "[LiveChatAI] Qwen 모델 로드 성공"
            );
        }
        catch (Exception e)
        {
            _llama = null;
            _isReady = false;

            Debug.LogError(
                $"[LiveChatAI] Qwen 모델 로드 실패: {e}"
            );
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
            Debug.LogWarning(
                "[LiveChatAI] AI가 준비되지 않았습니다. Fallback 사용."
            );

            onCompleted?.Invoke(null);
            return;
        }

        if (_isGenerating)
        {
            Debug.Log(
                "[LiveChatAI] 현재 AI 생성 중입니다. 이번 요청은 건너뜁니다."
            );

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

        string prompt =
            "<|im_start|>system\n" +
            "너는 한국 인터넷 먹방 방송의 시청자다.\n" +
            "방송 채팅창에 남길 짧은 반응만 작성한다.\n" +
            "반드시 자연스러운 한국어로 작성한다.\n" +
            "닉네임은 작성하지 않는다.\n" +
            "설명하지 않는다.\n" +
            "한 줄만 작성한다.\n" +
            "이모지와 이모티콘은 사용하지 않는다.\n" +
            $"5~{_maxCharacters}자 정도로 짧게 작성한다.\n" +
            "<|im_end|>\n" +
            "<|im_start|>user\n" +
            $"스트리머가 방금 {foodName}을(를) 먹었다. " +
            "시청자가 순간적으로 남길 법한 채팅 반응을 작성해라.\n" +
            "<|im_end|>\n" +
            "<|im_start|>assistant\n";

        Debug.Log(
            $"[LiveChatAI] Qwen 요청: {foodName}"
        );

        yield return null;

        try
        {
            result = _llama.Run(
                prompt,
                maxTokens: (uint)_maxTokens
            );
        }
        catch (Exception e)
        {
            generationException = e;
        }

        if (generationException != null)
        {
            Debug.LogError(
                $"[LiveChatAI] AI 생성 실패: {generationException}"
            );

            _isGenerating = false;

            onCompleted?.Invoke(null);
            yield break;
        }

        result = CleanChat(result);

        _isGenerating = false;

        if (string.IsNullOrWhiteSpace(result))
        {
            Debug.LogWarning(
                "[LiveChatAI] AI 결과가 비어 있습니다. Fallback 사용."
            );

            onCompleted?.Invoke(null);
            yield break;
        }

        Debug.Log(
            $"[LiveChatAI] AI 채팅: {result}"
        );

        onCompleted?.Invoke(result);
    }


    private string CleanChat(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        text = text.Trim();

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

        string[] lines =
            text.Split(
                new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries
            );

        if (lines.Length == 0)
            return null;

        text = lines[0].Trim();

        if (text.StartsWith("답변:"))
        {
            text = text.Substring(3).Trim();
        }

        if (text.StartsWith("Thinking", StringComparison.OrdinalIgnoreCase))
            return null;

        if (text.StartsWith("생각"))
            return null;

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

        if (string.IsNullOrWhiteSpace(text))
            return null;

        if (text.Length < _minCharacters)
            return null;

        if (text.Length > _maxCharacters)
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