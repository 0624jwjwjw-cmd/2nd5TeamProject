using UnityEngine;

public class SceneLoaderButton : MonoBehaviour
{
    public void LoadTitle()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        SceneLoader.Instance.LoadScene(SceneType.Title);
    }
    public void LoadMain()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        SceneLoader.Instance.LoadScene(SceneType.Main);
    }
    public void LoadStudio()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        SceneLoader.Instance.LoadScene(SceneType.Studio);

    }
    public void LoadKichen()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        SceneLoader.Instance.LoadScene(SceneType.Kitchen);
    }

}