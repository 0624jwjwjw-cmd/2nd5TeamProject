//**인벤토리 카테고리 버튼의 선택 테두리 표시만 담당**
using UnityEngine;

[DisallowMultipleComponent]
public sealed class InventoryCategoryTabUI : MonoBehaviour
{
    [Header("Category Selected Frames")]

    //전체 탭이 선택되었을 때 보일 테두리
    [SerializeField] private GameObject allSelectedFrame;

    //재료 탭이 선택되었을 때 보일 테두리
    [SerializeField] private GameObject ingredientSelectedFrame;

    //요리 탭이 선택되었을 때 보일 테두리
    [SerializeField] private GameObject dishSelectedFrame;

    //전달받은 카테고리에 맞춰 선택 테두리 상태를 갱신
    public void ShowSelected(InventoryCategory category)
    {
        SetFrameActive(allSelectedFrame, category == InventoryCategory.All);

        SetFrameActive(ingredientSelectedFrame, category == InventoryCategory.Ingredient);

        SetFrameActive(dishSelectedFrame, category == InventoryCategory.Dish);
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
