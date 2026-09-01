using UnityEngine;
using UnityEngine.UI;

public class RecipeBookUIManager : MonoBehaviour
{
    [SerializeField] private GameObject recipeBook;
    [SerializeField] private ScrollRect dishScrollRect;
    [SerializeField] private Image background;
    public void OnClickRecipeBookButton()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        recipeBook.SetActive(true);
        dishScrollRect.verticalNormalizedPosition = 1f;
        background.gameObject.SetActive(true);
    }
    public void OnClickExitButton()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        recipeBook.SetActive(false);
        background.gameObject.SetActive(false);
    }
}
