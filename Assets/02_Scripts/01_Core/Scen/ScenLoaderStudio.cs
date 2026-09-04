using UnityEngine;

public class ScenLoaderStudio : MonoBehaviour
{
    [SerializeField] private LiveManager liveManager;
    [SerializeField] private FoodArea foodArea;
    public void LoadMain()
    {
        if (liveManager.IsLive)
        {
            return;
        }
        if (foodArea != null)
        {
            foodArea.ReturnAllFoodToInventory();
        }
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        SceneLoader.Instance.LoadScene(SceneType.Main);
    }
}
