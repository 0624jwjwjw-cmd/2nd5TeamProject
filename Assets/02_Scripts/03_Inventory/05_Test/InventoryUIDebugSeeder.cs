//**실제 Inventory UI 확인을 위해
//임시 아이템을 InventoryManager에 넣는 디버그 스크립트**
//
//상점 시스템이 연결된 이후에는 삭제할 테스트용 코드
using UnityEngine;

//같은 GameObject에 Controller가 중복으로 붙는 것을 방지
[DisallowMultipleComponent]

public sealed class InventoryUIDebugSeeder : MonoBehaviour
{
    [Header("테스트할 Inventory Manager")] 
    [SerializeField] private InventoryManager inventoryManager;

    //기본 인벤토리 데이터 생성
    [ContextMenu("기본 UI 데이터 넣기")]
    public void AddDefaultItems()
    {
        //Play Mode에서만 실행
        if (!Application.isPlaying)
        {
            Debug.LogError(
                "[InventoryUIDebugSeeder] Play 모드에서 실행해주세요."
            );

            return;
        }

        //InventoryManager 연결 확인
        if (inventoryManager == null)
        {
            Debug.LogError(
                "[InventoryUIDebugSeeder] InventoryManager가 연결되지 않았습니다."
            );

            return;
        }

        //기존 테스트 데이터 초기화
        inventoryManager.ClearInventory();

        inventoryManager.AddItem("IG_01", 3);   //재료: 빵
        inventoryManager.AddItem("IG_04", 2);   //재료: 계란
        inventoryManager.AddItem("DS_01", 1);   //일반 요리
        inventoryManager.AddItem("SD_01", 1);   //특별 요리

        Debug.Log("[InventoryUIDebugSeeder] UI 확인용 아이템 추가 완료");
    }

    //부분 갱신 확인
    [ContextMenu("빵 +1")]
    public void AddBread()
    {
        if (!CanRun()) return;

        inventoryManager.AddItem("IG_01", 1);
    }

    //전체 초기화
    [ContextMenu("인벤토리 비우기")]
    public void ClearInventory()
    {
        if (!CanRun()) return;

        inventoryManager.ClearInventory();
    }

    //공통 실행 검사
    private bool CanRun()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("[InventoryUIDebugSeeder] Play 모드에서 실행해주세요.");
            return false;
        }


        if (inventoryManager == null)
        {
            Debug.LogError("[InventoryUIDebugSeeder] InventoryManager가 연결되지 않았습니다.");
            return false;
        }

        return true;
    }
}
