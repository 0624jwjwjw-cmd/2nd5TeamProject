using UnityEngine;

public class ScenLoaderStudio : MonoBehaviour
{
    [SerializeField] private LiveManager liveManager;
    public void LoadMain()
    {
        if (liveManager.IsLive)
        {
            return;
        }
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        SceneLoader.Instance.LoadScene(SceneType.Main);
    }
}
