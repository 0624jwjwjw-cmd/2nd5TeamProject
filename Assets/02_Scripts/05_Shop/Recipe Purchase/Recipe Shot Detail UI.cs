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

    private ReciepePurchaseData reciepePurchaseData;
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
        priceText.text = reciepePurchaseData.Price.ToString();
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
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        //후원금 보유량 체크, 감소로직 추가 필요
        ReciepeUnlockManager.Instance.UnlockRecipe(reciepePurchaseData.FoodID);
        purchaseButton.interactable = false;
        purchaseButtonText.text = completePurchase;
    }
}
