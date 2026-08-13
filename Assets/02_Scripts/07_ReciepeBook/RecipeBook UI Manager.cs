using UnityEngine;
using UnityEngine.UI;

public class RecipeBookUIManager : MonoBehaviour
{
    [SerializeField] private GameObject recipeBook;
    [SerializeField] private ScrollRect dishScrollRect;

    public void OnClickRecipeBookButton()
    {
        recipeBook.SetActive(true);
        dishScrollRect.verticalNormalizedPosition = 1f;
        
    }
    public void OnClickExitButton()
    {
        recipeBook.SetActive(false);
    }
}
