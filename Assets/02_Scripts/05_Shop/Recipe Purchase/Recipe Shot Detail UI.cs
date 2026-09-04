using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeShotDetailUI : MonoBehaviour
{
    [SerializeField] private Image foodImage;
    [SerializeField] private TMP_Text foodNameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text itemInfoText;
    [SerializeField] private TMP_Text recipeInfoText;

    [SerializeField] private Button purchaseButton;
    [SerializeField] private TMP_Text purchaseButtonText;

    [SerializeField] private string tryPurchase = "구매하기";
    [SerializeField] private string completePurchase = "구매완료";

    [SerializeField] private TMP_Text warningText;

    private ReciepePurchaseData reciepePurchaseData;
    private void OnEnable()
    {
        warningText.gameObject.SetActive(false);
    }
    public void OpenDetail(ReciepePurchaseData reciepePurchaseData)
    {
        this.reciepePurchaseData = reciepePurchaseData;
        gameObject.SetActive(true);
        if(ItemVisualRepository.Instance.TryGetIcon(reciepePurchaseData.FoodID, out Sprite icon))
        {
            foodImage.sprite = icon;
        }
        if(GameDataRepository.Instance.TryGetDish(reciepePurchaseData.FoodID, out DishData dishData))
        {
            foodNameText.text = dishData.DishName;
        }
        priceText.text = reciepePurchaseData.Price.ToString() + "원";
        itemInfoText.text = reciepePurchaseData.Info;
        recipeInfoText.text = reciepePurchaseData.ReciepeInfo;

        if(ReciepeUnlockManager.Instance.IsUnlocked(reciepePurchaseData.FoodID))
        {
            purchaseButton.interactable = false;
            purchaseButtonText.text = completePurchase;
        }
        else
        {
            purchaseButton.interactable = true;
            purchaseButtonText.text = tryPurchase;
        }
    }
    public void CloseDetail()
    {
        gameObject.SetActive(false);
    }
    public void OnclickPurchaseButton()
    {
        if (!CurrencyManager.Instance.SpendGold(reciepePurchaseData.Price))
        {
            warningText.text = (reciepePurchaseData.Price - CurrencyManager.Instance.Gold).ToString() + "원 부족합니다.";
            warningText.gameObject.SetActive(true);
            SoundManager.Instance?.PlaySFX(SFXType.Lose);
            return;
        }
        warningText.gameObject.SetActive(false);
        SoundManager.Instance?.PlaySFX(SFXType.Coin);
        ReciepeUnlockManager.Instance.UnlockRecipe(reciepePurchaseData.FoodID);
        purchaseButton.interactable = false;
        purchaseButtonText.text = completePurchase;
    }
}
