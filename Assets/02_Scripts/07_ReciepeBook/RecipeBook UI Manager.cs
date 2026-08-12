using UnityEngine;

public class RecipeBookUIManager : MonoBehaviour
{
    [SerializeField] private GameObject recipeBook;

    public void OnClickRecipeBookButton()
    {
        recipeBook.SetActive(true);
    }
    public void OnClickExitButton()
    {
        recipeBook.SetActive(false);
    }
}
