using UnityEngine;

public class SceneLoaderButton : MonoBehaviour
{
    public void LoadTitle()
    {
        SceneLoader.Instance.LoadScene(SceneType.Title);
    }
    public void LoadMain()
    {
        SceneLoader.Instance.LoadScene(SceneType.Main);
    }
    public void LoadStudio()
    {
        SceneLoader.Instance.LoadScene(SceneType.Studio);
    }
    public void LoadKichen()
    {
        SceneLoader.Instance.LoadScene(SceneType.Kitchen);
    }
    public void LoadShop()
    {
        SceneLoader.Instance.LoadScene(SceneType.Shop);
    }
}