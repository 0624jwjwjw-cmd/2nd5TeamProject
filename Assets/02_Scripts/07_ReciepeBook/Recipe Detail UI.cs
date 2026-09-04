using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RecipeDetailUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite emptyDish;
    [SerializeField] private TMP_Text foodNameText;
    [SerializeField] private TMP_Text recipeTitleText;
    [SerializeField] private TMP_Text[] materialName;
    [SerializeField] private TMP_Text[] materialCount;
    [SerializeField] private Image plus1;
    [SerializeField] private Image plus2;
    [SerializeField] private TMP_Text infoTitleText;
    [SerializeField] private TMP_Text infoText;

    [SerializeField] private Image lockedImage;
    [SerializeField] private Color originColor = new Color(255f, 255f, 255f);
    [SerializeField] private Color lockedColor = new Color(0f, 0f, 0f);
    [SerializeField] private string questionMark = "???";
    [SerializeField] private TMP_Text lockedRecipeText;

    [SerializeField] private Image dotedLine1;
    private void Awake()
    {
        ResetDish();
    }
    private void OnEnable()
    {
        ResetDish();
    }
    public void LockedDish(DishData dishData)
    {
        dotedLine1.gameObject.SetActive(false);
        if (ItemVisualRepository.Instance.TryGetIcon(dishData.ID, out Sprite icon))
        {
            image.sprite = icon;
        }
        image.color = lockedColor;
        lockedImage.gameObject.SetActive(true);
        foodNameText.text = questionMark;
        recipeTitleText.gameObject.SetActive(false);
        for (int i = 0; i < materialName.Length; i++) materialName[i].text = "";
        for (int i = 0; i < materialCount.Length; i++) materialCount[i].text = "";
        plus1.gameObject.SetActive(false);
        plus2.gameObject.SetActive(false);
        infoTitleText.gameObject.SetActive(false);
        infoText.text = "";
        lockedRecipeText.gameObject.SetActive(true);
    }
    public void UnlockedDish(DishData dishData)
    {
        dotedLine1.gameObject.SetActive(true);
        for (int i = 0; i < materialName.Length; i++) materialName[i].text = "";
        for (int i = 0; i < materialCount.Length; i++) materialCount[i].text = "";
        lockedImage.gameObject.SetActive(false);
        lockedRecipeText.gameObject.SetActive(false);
        if (ItemVisualRepository.Instance.TryGetIcon(dishData.ID, out Sprite icon))
        {
            image.sprite = icon;
        }
        image.color = originColor;
        foodNameText.text = dishData.DishName;
        recipeTitleText.gameObject.SetActive(true);
        for (int i = 0; i < dishData.Materials.Length; i++)
        {
            materialName[i].text = dishData.Materials[i].IngredientData.IngredientName;
            materialCount[i].text = dishData.Materials[i].Amount.ToString();
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
        infoTitleText.gameObject.SetActive(true);
        infoText.text = dishData.Info;
    }
    public void ResetDish()
    {
        image.sprite = emptyDish;
        foodNameText.text = "";
        recipeTitleText.gameObject.SetActive(false);
        for (int i = 0; i < materialName.Length; i++)
        {
            materialName[i].text = "";
        }
        for (int i= 0; i< materialCount.Length;i++)
        {
            materialCount[i].text = "";
        }
        plus1.gameObject.SetActive(false);
        plus2.gameObject.SetActive(false);
        infoTitleText.gameObject.SetActive(false);
        infoText.text = "";
        lockedImage.gameObject.SetActive(false);
        lockedRecipeText.gameObject.SetActive(false);
        dotedLine1.gameObject.SetActive(false);
    }
}
