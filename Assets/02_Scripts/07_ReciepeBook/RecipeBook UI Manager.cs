using UnityEngine;
using UnityEngine.UI;

public class RecipeBookUIManager : MonoBehaviour
{
    [SerializeField] private GameObject recipeBook;
    [SerializeField] private ScrollRect dishScrollRect;
    [SerializeField] private Image background;
    public void OnClickRecipeBookButton()
    {
        recipeBook.SetActive(true);
        dishScrollRect.verticalNormalizedPosition = 1f;
        background.gameObject.SetActive(true);
    }
    public void OnClickExitButton()
    {
        recipeBook.SetActive(false);
        background.gameObject.SetActive(false);
    }
}
