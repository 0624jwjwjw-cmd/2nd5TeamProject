using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RecipeDetailUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text foodNameText;
    [SerializeField] private TMP_Text[] materialName;
    [SerializeField] private TMP_Text[] materialCount;
    [SerializeField] private Image plus1;
    [SerializeField] private Image plus2;
    [SerializeField] private TMP_Text infoText;

    [SerializeField] private GameObject lockedImage;
    [SerializeField] private Color originColor;
    [SerializeField] private Color lockedColor = new Color(0f, 0f, 0f);
    [SerializeField] private string questionMark = "???";
    [SerializeField] private string lockedRecipe = "레시피가 해금되지 않았습니다.";
    [SerializeField] private TMP_Text lockedRecipeText;
    [SerializeField] private string lockedInfo = "레시피를 구매하여 요리를 해금해 주세요.";
    [SerializeField] private TMP_Text lockedInfoText;

    public void LockedDish(DishBase dishBase)
    {
        image.sprite = dishBase.spriteRenderer.sprite;
        image.color = lockedColor;
        lockedImage.SetActive(true);
        foodNameText.text = questionMark;
        for (int i = 0; i < materialName.Length; i++)
        {
            materialName[i].text = "";
        }
        for (int i = 0; i < materialCount.Length; i++)
        {
            materialCount[i].text = "";
        }
        plus1.gameObject.SetActive(false);
        plus2.gameObject.SetActive(false);
        lockedRecipeText.gameObject.SetActive(true);
        lockedRecipeText.text = lockedRecipe;
        lockedInfoText.gameObject.SetActive(true);
        lockedInfoText.text = lockedInfo;
    }
    public void UnlockedDish(DishBase dishBase)
    {
        lockedImage.SetActive(false);
        image.sprite = dishBase.spriteRenderer.sprite;
        foodNameText.text = dishBase.DishName;
        for (int i = 0; i < dishBase.Materials.Length; i++)
        {
            materialName[i].text = dishBase.Materials[i].IngredientData.IngredientName;
            materialCount[i].text = dishBase.Materials[i].Amount.ToString();
        }

        if (string.IsNullOrEmpty(materialName[1].text))
        {
            plus1.gameObject.SetActive(false);
            plus2.gameObject.SetActive(false);
        }
        else if (!string.IsNullOrEmpty(materialName[1].text) && string.IsNullOrEmpty(materialName[2].text))
        {
            plus1.gameObject.SetActive(true);
            plus2.gameObject.SetActive(false);
        }
        else if (!string.IsNullOrEmpty(materialName[1].text) && !string.IsNullOrEmpty(materialName[2].text))
        {
            plus1.gameObject.SetActive(true);
            plus2.gameObject.SetActive(true);
        }
        infoText.text = dishBase.Info;
    }
}
