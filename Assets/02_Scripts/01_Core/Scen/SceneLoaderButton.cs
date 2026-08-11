using UnityEngine;

public class SceneLoaderButton : MonoBehaviour
{
    public void LoadTitle()
    {
        SceneLoader.Instance.LoadScene(SceneType.Title);
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
    }
    public void LoadMain()
    {
        SceneLoader.Instance.LoadScene(SceneType.Main);
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
    }
    public void LoadStudio()
    {
        SceneLoader.Instance.LoadScene(SceneType.Studio);
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
    }
    public void LoadKichen()
    {
        SceneLoader.Instance.LoadScene(SceneType.Kitchen);
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
    }
    public void LoadShop()
    {
        SceneLoader.Instance.LoadScene(SceneType.Shop);
    }
    public void LoadTest()
    {
        SceneLoader.Instance.LoadScene(SceneType.Test);
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
    }
    public void SoundTest()
    {
        SoundManager.Instance.PlaySFX(SFXType.Coin);
    }
}