using UnityEngine;
using UnityEngine.UI;

public class RecipeShopUIController : MonoBehaviour
{
    [SerializeField] private GameObject recipePurchasePanel;
    [SerializeField] private Button recipeShopOpenButton;

    public void OnclickRecipeShopOpenButton()
    {
        recipePurchasePanel.gameObject.SetActive(true);
        recipeShopOpenButton.gameObject.SetActive(false);
    }
    public void OnclickRecipeShopExitButton()
    {
        recipePurchasePanel.gameObject.SetActive(false);
        recipeShopOpenButton.gameObject.SetActive(true);
    }
}
