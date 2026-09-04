using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeBookUIManager : MonoBehaviour
{
    [SerializeField] private GameObject recipeBook;
    [SerializeField] private ScrollRect dishScrollRect;
    [SerializeField] private Image background;

    [SerializeField] private RecipeBookSlotUI slotPrefab;
    [SerializeField] private Transform slotParent;
    [SerializeField] private Transform poolRoot;
    [SerializeField] private RecipeDetailUI recipeDetailUI;

    private ComponentPool<RecipeBookSlotUI> slotPool;
    private readonly List<RecipeBookSlotUI> activeSlots = new();
    private bool hasPopulated = false;

    public void OnClickRecipeBookButton()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);

        if (!hasPopulated)
        {
            PopulateSlots();
            hasPopulated = true;
        }

        recipeBook.SetActive(true);
        background.gameObject.SetActive(true);
        StartCoroutine(ResetScrollCo());
    }
    public void OnClickExitButton()
    {
        SoundManager.Instance.PlaySFX(SFXType.ButtonClick);
        recipeBook.SetActive(false);
        background.gameObject.SetActive(false);
    }
    private IEnumerator ResetScrollCo()
    {
        yield return null;
        dishScrollRect.verticalNormalizedPosition = 1f;
    }
    private void PopulateSlots()
    {
        slotPool = new ComponentPool<RecipeBookSlotUI>(slotPrefab, poolRoot);

        foreach (DishData dishData in GameDataRepository.Instance.dishLookup.Values)
        {
            RecipeBookSlotUI slot = slotPool.Get(slotParent);
            slot.Setup(dishData, recipeDetailUI);
            activeSlots.Add(slot);
        }
    }
}