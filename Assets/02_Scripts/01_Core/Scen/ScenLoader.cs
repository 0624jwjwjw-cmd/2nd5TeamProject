using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void LoadScene(SceneType sceneType)
    {
        string sceneName = GetSceneName(sceneType);

        SceneManager.LoadScene(sceneName);
    }


    private string GetSceneName(SceneType sceneType)
    {
        switch (sceneType)
        {
            case SceneType.Title:
                return "00_Title";

            case SceneType.Main:
                return "01_Main_Living";

            case SceneType.Studio:
                return "02_Studio";

            case SceneType.Kitchen:
                return "03_Kitchen";

            case SceneType.Shop:
                return "04_Shop";

            case SceneType.Test:
                return "Test";

            default:
                return "00_Title";
        }
    }
}