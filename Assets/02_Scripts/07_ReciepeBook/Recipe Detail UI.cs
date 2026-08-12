using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RecipeDetailUI : MonoBehaviour
{
    public static RecipeDetailUI Instance { get; private set; }

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

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ResetDish();
    }
    private void OnEnable()
    {
        ResetDish();
    }
    public void LockedDish(DishBase dishBase)
    {
        image.sprite = dishBase.spriteRenderer.sprite;
        image.color = lockedColor;
        lockedImage.gameObject.SetActive(true);
        foodNameText.text = questionMark;
        recipeTitleText.gameObject.SetActive(false);
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
        infoTitleText.gameObject.SetActive(false);
        infoText.text = "";
        lockedRecipeText.gameObject.SetActive(true);

    }
    public void UnlockedDish(DishBase dishBase)
    {
        for (int i = 0; i < materialName.Length; i++)
        {
            materialName[i].text = "";
        }
        for (int i = 0; i < materialCount.Length; i++)
        {
            materialCount[i].text = "";
        }
        lockedImage.gameObject.SetActive(false);
        lockedRecipeText.gameObject.SetActive(false);
        image.sprite = dishBase.spriteRenderer.sprite;
        image.color = originColor;
        foodNameText.text = dishBase.DishName;
        recipeTitleText.gameObject.SetActive(true);
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
        infoTitleText.gameObject.SetActive(true);
        infoText.text = dishBase.Info;
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
    }
}
