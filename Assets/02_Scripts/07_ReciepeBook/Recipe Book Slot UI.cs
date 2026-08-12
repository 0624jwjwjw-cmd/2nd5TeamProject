using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RecipeBookSlotUI : MonoBehaviour
{
    [SerializeField] private DishBase dishBase;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Color originColor;
    [SerializeField] private Color black = new Color(0f, 0f, 0f);
    [SerializeField] private RecipeDetailUI recipeDetailUI;
    private void Awake()
    {
        image.sprite = dishBase.spriteRenderer.sprite;
        originColor = dishBase.spriteRenderer.color;

        RecipeStateManage();
    }
    private void Update()
    {
        RecipeStateManage();
    }
    private void RecipeStateManage()
    {
        if (!ReciepeUnlockManager.Instance.IsUnlocked(dishBase.ID))
        {
            image.color = black;
            text.text = "???";
        }
        else
        {
            image.color = originColor;
            text.text = dishBase.DishName;
        }
    }
    public void OnClickDishIcon()
    {
        if (!ReciepeUnlockManager.Instance.IsUnlocked(dishBase.ID))
        {
            recipeDetailUI.LockedDish(dishBase);
        }
        else
        {
            recipeDetailUI.UnlockedDish(dishBase);
        }
    }

}
