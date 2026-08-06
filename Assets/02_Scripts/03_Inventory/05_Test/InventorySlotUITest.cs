//**인벤토리 슬롯 UI 테스트용**
using UnityEngine;

//테스트 컴포넌트 여러 개 추가 방지
[DisallowMultipleComponent]
public sealed class InventorySlotUITest : MonoBehaviour
{
    [Header("테스트 대상 슬롯")]

    //테스트할 InventorySlotUI 컴포넌트를 연결
    [SerializeField] private InventorySlotUI targetSlot;

    [Header("테스트용 아이템 정보")]
    [SerializeField] private string testItemId = "DS_01";                       //빵 재료 사용용
    [SerializeField] private string testDisplayName = "식빵";                   //슬롯에 표시할 이름
    [SerializeField] private Sprite testIcon;                                   //슬롯에 표시할 테스트용 아이콘
    [SerializeField] private int testAmount = 12;                               //슬롯에 표시할 테스트 수량

    private bool isSelected;    //현재 슬롯이 선택된 상태인지
    void Start()
    {
        ApplyTestData();        //테스트 데이터 슬롯에 표시
    }

    //컴포넌트 점 세 개 메뉴에서 테스트 다시 적용 가능하게
    [ContextMenu("슬롯 UI 테스트 데이터 적용")]

    //[테스트용 데이터를 슬롯에 적용하는 핵심 메서드]
    public void ApplyTestData()
    {
        //테스트할 슬롯이 연결되어 있는지 확인
        if (targetSlot == null)
        {
            Debug.LogError(
                "[InventorySlotUITest] Target Slot이 연결되지 않았습니다."
                );
            return;
        }

        //수량이 1개 미만이면 테스트가 어려우니 최소 수량인 1개로 보정
        if (testAmount <= 0)
        {
            testAmount = 1;
        }

        isSelected = false; //슬롯 선택 상태 초기화

        //InventorySlotUI의 SetUp 메서드 호출
        targetSlot.Setup(
            testItemId,         //ID
            testIcon,           //아이콘
            testDisplayName,    //이름
            testAmount,         //수량
            HandleSlotClicked   //클릭 콜백
            );

        //테스트 데이터가 적용됐다는 사실을 콘솔에 출력
        Debug.Log(
            $"[InventorySlotUITest] 적용 완료 - " +
            $"ID: {testItemId}, 이름: {testDisplayName}, 수량: {testAmount}"
            );
    }

    //Inspector의 점 세 개 메뉴에서 수량 감소 테스트
    [ContextMenu("수량 1 감소")]

    //[현재 수량 1개 감소 메서드]
    public void DecreaseAmount()
    {
        //슬롯 연결 안되면 실행하지 않음
        if (targetSlot == null)
        {
            Debug.LogError("[InventorySlotUITest] Target Slot이 연결되지 않았습니다.");
            return;
        }

        //테스트 수량 1개 감소
        testAmount--;

        //감소한 수량을 슬롯 UI에 전달 (수량 0 이하면 InventorySlotUI가 슬롯을 자동으로 비움)
        targetSlot.UpdateAmount(testAmount);

        Debug.Log($"[InventorySlotUITest] 현재 테스트 수량: {testAmount}");
    }

    //Inspector의 점 세 개 메뉴에서 슬롯 비우기 테스트
    [ContextMenu("슬롯 비우기")]

    //[슬롯에 표시된 모든 정보 초기화하는 메서드]
    public void ClearTestSlot()
    {
        //슬롯 연결 안되면 실행하지 않음
        if (targetSlot == null)
        {
            Debug.LogError("[InventoryUITest] Target Slot이 연결되지 않았습니다.");
            return;
        }

        //슬롯의 ID, 아이콘, 이름, 수량, 콜백을 모두 초기화
        targetSlot.ClearSlot();

        //테스트 스크립트가 기억하는 선택 상태도 초기화
        isSelected = false;

        //슬롯을 비웠다는 사실 출력
        Debug.Log("[InventorySlotUITest] 슬롯 비우기 완료");
    }

    //[InventroySlotUI가 클릭됐을 때 호출되는 콜백 메서드]
    private void HandleSlotClicked(string clickedItemId)
    {
        isSelected = !isSelected;   //현재 상태 반대로

        //선택 상태에 맞게 SelectedFrame을 켜거나 끔
        targetSlot.SetSelected(isSelected);

        //어떤 아이템이 클릭됐는지 Console에 출력
        Debug.Log(
            $"[InventorySlotUITest] 클릭 ID: {clickedItemId}, " +
            $"선택 상태: {isSelected}"
            );
    }
}
