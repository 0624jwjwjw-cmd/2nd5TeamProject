using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(IngredientBase))]
public class IngredientSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CookSlots cookSlots;
    private IngredientBase ingredientBase;

    private void Awake()
    {
        ingredientBase = GetComponent<IngredientBase>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ingredientBase == null || ingredientBase.Data == null) return;

        // TODO(인벤토리 연동 시): 여기서 실제 보유 개수 확인 후 부족하면 return;
        // if (!InventoryManager.Instance.TryUseIngredient(ingredientBase.Data, 1)) return;

        cookSlots.AddIngredient(ingredientBase.Data);

        // TODO(인벤토리 연동 시): 개수 텍스트 갱신
        // countText.text = InventoryManager.Instance.GetCount(ingredientBase.Data).ToString();
    }
}
