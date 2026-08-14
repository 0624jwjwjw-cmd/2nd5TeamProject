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
    private void Awake()
    {
        recipeNameText.text = reciepePurchaseData.ReciepeName;
        recipePriceText.text = reciepePurchaseData.Price.ToString() + "원";
    }
    private void OnEnable()
    {
        StartCoroutine(WaitManager());
    }
    public void OnClickRecipePurchaseButton()
    {
        //후원금 감소로직
        ReciepeUnlockManager.Instance.UnlockRecipe(reciepePurchaseData.FoodID);
        purchaseButton.interactable = false;
        purchaseButtonText.text = completePurchase;

        //
        HashSet<string> unlockedRecipes = ReciepeUnlockManager.Instance.unlockedRecipeIDs;
        foreach(string recipeIDs in unlockedRecipes)
        {
            Debug.Log($"해제된 레시피 : {recipeIDs}");
        }
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
}
