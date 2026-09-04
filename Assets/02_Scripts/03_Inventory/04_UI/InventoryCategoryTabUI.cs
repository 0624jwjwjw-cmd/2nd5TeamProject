//**인벤토리 카테고리 버튼에서 선택하지 않은 탭을 어둡게 표시하는 UI**
using UnityEngine;

[DisallowMultipleComponent]
public sealed class InventoryCategoryTabUI : MonoBehaviour
{
    [Header("카테고리 비선택 어둡게 표시")]

    //전체 탭이 선택되지 않았을 때 보일 어두운 오버레이
    [SerializeField] private GameObject allSelectedFrame;

    //재료 탭이 선택되지 않았을 때 보일 어두운 오버레이
    [SerializeField] private GameObject ingredientSelectedFrame;

    //요리 탭이 선택되지 않았을 때 보일 어두운 오버레이
    [SerializeField] private GameObject dishSelectedFrame;

    //전달받은 카테고리에 맞춰 선택 테두리 상태를 갱신
    public void ShowSelected(InventoryCategory category)
    {
        SetFrameActive(allSelectedFrame, category != InventoryCategory.All);

        SetFrameActive(ingredientSelectedFrame, category != InventoryCategory.Ingredient);

        SetFrameActive(dishSelectedFrame, category != InventoryCategory.Dish);
    }

    //Frame이 연결되어 있을 때만 활성화 상태 변경
    private void SetFrameActive(GameObject targetFrame, bool isActive)
    {
        if (targetFrame != null)
        {
            targetFrame.SetActive(isActive);
        }
    }
}
