using UnityEngine;
using UnityEngine.UI;

public class RecipeShopUIController : MonoBehaviour
{
    [SerializeField] private GameObject recipePurchasePanel;
    [SerializeField] private Button recipeShopOpenButton;

    public void OnclickRecipeShopOpenButton()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        recipePurchasePanel.gameObject.SetActive(true);
        recipeShopOpenButton.gameObject.SetActive(false);
    }
    public void OnclickRecipeShopExitButton()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        recipePurchasePanel.gameObject.SetActive(false);
        recipeShopOpenButton.gameObject.SetActive(true);
    }
}
