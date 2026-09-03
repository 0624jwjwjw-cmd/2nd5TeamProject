//**인벤토리에서 선택한 아이템의 이름과 설명을 표시하는 UI 전용 스크립트**
using TMPro;
using UnityEngine;

//같은 GameObject에 이 컴포넌트가 여러 개 붙지 않게 하기
[DisallowMultipleComponent]
public sealed class InventoryItemDescriptionUI : MonoBehaviour
{
    [Header("Item Description UI")]
    [SerializeField] private TMP_Text itemNameText;     //선택한 아이템 이름 표시
    [SerializeField] private TMP_Text descriptionText;  //선택한 아이템 설명 표시

    //Controller가 전달한 이름과 설명을 화면에 표시
    public void Show(string itemName, string description)
    {
        //이름 Text가 연결되어 있을 때만 이름 변경
        if (itemNameText != null)
        {
            itemNameText.text = itemName;
        }

        //설명 Text가 연결되어 있을 때만 설명 변경
        if (descriptionText != null)
        {
            descriptionText.text = description;
        }
    }

    //현재 표시 중인 이름과 설명을 모두 비움
    public void Clear()
    {
        //Show를 다시 사용하므로 Text를 비우는 코드가 중복되지 않음
        Show(string.Empty, string.Empty);
    }
}
