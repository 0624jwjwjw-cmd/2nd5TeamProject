using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipePurchaseSlot : MonoBehaviour
{
    [SerializeField] public ReciepePurchaseData reciepePurchaseData;

    [SerializeField] private TMP_Text recipeNameText;
    [SerializeField] private TMP_Text recipePriceText;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TMP_Text purchaseButtonText;

    [SerializeField] private string tryPurchase = "구매하기";
    [SerializeField] private string completePurchase = "구매완료";

    [SerializeField] private RecipeShotDetailUI recipeShotDetailUI;

    [SerializeField] private TMP_Text warningText;
    private void Awake()
    {
        recipeNameText.text = reciepePurchaseData.ReciepeName;
        recipePriceText.text = reciepePurchaseData.Price.ToString() + "원";
    }
    private void OnEnable()
    {
        StartCoroutine(WaitManager());
        ReciepeUnlockManager.Instance.OnUnlockChanged += CheckPurchase;
    }
    private void OnDisable()
    {
        ReciepeUnlockManager.Instance.OnUnlockChanged -= CheckPurchase;
    }
    public void OnClickRecipePurchaseButton()
    {
        if(!CurrencyManager.Instance.SpendGold(reciepePurchaseData.Price))
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
    private void CheckPurchase()
    {

        if (ReciepeUnlockManager.Instance.IsUnlocked(reciepePurchaseData.FoodID))
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
    private IEnumerator WaitManager()
    {
        while(ReciepeUnlockManager.Instance == null)
        {
            yield return null;
        }
        CheckPurchase();
    }
    public void OnclickRecipeImage()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        recipeShotDetailUI.OpenDetail(reciepePurchaseData);
    }
}
