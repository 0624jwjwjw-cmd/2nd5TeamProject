using UnityEngine;
using LlamaCppUnity;
using System.IO;

public class LlamaTest : MonoBehaviour
{
    private Llama _llama;

    private void Start()
    {
        string modelPath = Path.Combine(
            Application.streamingAssetsPath,
            "Models",
            "qwen2.5-0.5b-instruct-q4_k_m.gguf"
        );

        Debug.Log("Model Path: " + modelPath);

        try
        {
            _llama = new Llama(modelPath);

            string result = _llama.Run(
                "한국어로 짧게 인사해줘.",
                maxTokens: 32
            );

            Debug.Log("AI RESULT: " + result);
        }
        catch (System.Exception e)
        {
            Debug.LogError("AI FAILED: " + e);
        }
    }
}