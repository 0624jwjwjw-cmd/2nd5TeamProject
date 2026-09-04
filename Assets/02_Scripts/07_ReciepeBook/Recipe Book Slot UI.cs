using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeBookSlotUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Color originColor = new Color(1f,1f,1f,1f);
    [SerializeField] private Color black = new Color(0f, 0f, 0f);

    private DishData dishData;
    private RecipeDetailUI recipeDetailUI;

    public void Setup(DishData dishData, RecipeDetailUI recipeDetailUI)
    {
        this.dishData = dishData;
        this.recipeDetailUI = recipeDetailUI;

        if (ItemVisualRepository.Instance.TryGetIcon(dishData.ID, out Sprite icon))
        {
            image.sprite = icon;
        }

        RecipeStateManage();
    }
    private void OnEnable()
    {
        if (dishData != null)
        {
            RecipeStateManage();
        }
    }
    private void RecipeStateManage()
    {
        if (!ReciepeUnlockManager.Instance.IsUnlocked(dishData.ID))
        {
            image.color = black;
            text.text = "???";
        }
        else
        {
            image.color = originColor;
            text.text = dishData.DishName;
        }
    }
    public void OnClickDishIcon()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        if (!ReciepeUnlockManager.Instance.IsUnlocked(dishData.ID))
        {
            recipeDetailUI.LockedDish(dishData);
        }
        else
        {
            recipeDetailUI.UnlockedDish(dishData);
        }
    }
}