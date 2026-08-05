//**인벤토리 매니저 테스트용**
using UnityEngine;

//테스트 컴포넌트 여러 개 추가 방지
[DisallowMultipleComponent]
public sealed class InventoryManagerTest : MonoBehaviour
{
    [Header("테스트용 재료 ID")]
    [SerializeField] private string breadIngredientId = "IG_01";    //재료 빵 테스트
    [SerializeField] private string eggIngredientId = "IG_04";      //재료 계란 테스트

    [Header("테스트용 음식 ID")]
    [SerializeField] private string whiteBreadDishId = "FD_01";          //요리 식빵 테스트

    private InventoryManager inventoryManager;

    private void Start()
    {
        TryFindInventoryManager();
    }

    [ContextMenu("인벤토리 전체 테스트 실행")]

    public void RunAllTests()
    {
        if (!TryFindInventoryManager()) return;

        Debug.Log("========== 인벤토리 테스트 시작 ==========");

        inventoryManager.ClearInventory();  //인벤토리 초기화
        TestAddItem();                      //추가 수량 합산 테스트
        TestHasItem();                      //특정 수량 보유 여부 테스트
        TestRemoveItem();                   //수량 제거 및 빈 슬롯 삭제
        TestMaxStack();                     //한 슬롯 최대 스택 테스트
        TestIngredientAndDish();            //재료와 음식이 서로 다른 슬롯에 저장되는지 테스트

        Debug.Log("========== 인벤토리 테스트 종료 ==========");
    }

    //[슬롯 하나에 수량 합쳐지는지 테스트]
    private void TestAddItem()
    {
        //빵 재료 5개 추가
        bool firstAddResult = inventoryManager.AddItem(breadIngredientId, 5);

        //추가 성공 후 실제 보유량 5개인지 검사
        PrintTestResult("빵 재료 5개 추가", firstAddResult && inventoryManager.GetItemCount(breadIngredientId) == 5);

        //빵 재료 3개 더 추가
        bool secondAddResult = inventoryManager.AddItem(breadIngredientId, 3);

        //빵 재료의 총 수량이 8개이고 같은 아이템이므로 슬롯은 1개인지 검사
        PrintTestResult(
            "같은 빵 재료 3개 추가 후 총 8개",
            secondAddResult &&
            inventoryManager.GetItemCount(breadIngredientId) == 8 &&
            inventoryManager.SlotCount == 1
            );
    }

    //[아이템 필요한 수량만큼 보유 여부 테스트]
    private void TestHasItem()
    {
        //현재 빵 재료는 8개이므로 8개 보유 검사시 true가 나와야함
        bool hasEightBread = inventoryManager.HasItem(breadIngredientId, 8);

        //빵 재료 8개 보유한 게 맞으면
        PrintTestResult("빵 재료 8개 보유 확인", hasEightBread);

        //현재 빵 재료 8개이므로 9개 보유 검사는 false가 나와야함
        bool hasNineBread = inventoryManager.HasItem(breadIngredientId, 9);

        PrintTestResult("빵 재료 9개 미보유 확인", !hasNineBread);
    }

    //[아이템 제거, 수량 부족, 빈 슬롯 삭제 테스트]
    private void TestRemoveItem()
    {
        //빵 재료 2개 제거 (8개 - 2개)
        bool removeTwoResult = inventoryManager.RemoveItem(breadIngredientId, 2);

        //6개 남았는지 검사
        PrintTestResult(
            "빵 재료 2개 제거 후 6개", 
            removeTwoResult &&
            inventoryManager.GetItemCount(breadIngredientId) == 6
            );

        //보유량 보다 많은 10개 제거 시도
        bool removeTooManyResult = inventoryManager.RemoveItem(breadIngredientId, 10);

        PrintTestResult(
            "보유 수량보다 많이 제거하면 실패",
            !removeTooManyResult &&
            inventoryManager.GetItemCount(breadIngredientId) == 6
            );

        //남은 빵 재료 6개 모두 제거
        bool removeAllResult = inventoryManager.RemoveItem(breadIngredientId, 6);

        //제거 성공했고 수량 0이고 비어있는 슬롯까지 리스트에서 삭제됐는지 검사
        PrintTestResult(
            "빵 재료 전부 제거 후 슬롯 삭제",
            removeAllResult &&
            inventoryManager.GetItemCount(breadIngredientId) == 0 &&
            inventoryManager.SlotCount == 0
            );
    }

    //[슬롯당 최대 스택 수량 99개 제한 테스트]
    private void TestMaxStack()
    {
        //이전 테스트 데이터가 영향을 주지 않도록 초기화
        inventoryManager.ClearInventory();

        //빵 재료를 최대 수량인 99개 추가
        bool addMaxResult = inventoryManager.AddItem(breadIngredientId, 99);

        //99개 추가 성공했고 실제 보유량도 99개인지 검사
        PrintTestResult(
            "빵 재료 99개 추가",
            addMaxResult &&
            inventoryManager.GetItemCount(breadIngredientId) == 99
            );

        //99개가 들어있는 상태에서 1개 더 추가
        bool addOverflowResult = inventoryManager.AddItem(breadIngredientId, 1);

        //최대 수량 초과로 추가가 실패하고 기존 수량 99개가 유지 됐는지 검사
        PrintTestResult(
            "99개 초과 추가 차단",
            !addOverflowResult &&
            inventoryManager.GetItemCount(breadIngredientId) == 99
            );
    }

    //[서로 다른 재료와 음식이 각각 별도의 슬롯에 저장되는지 테스트]
    private void TestIngredientAndDish()
    {
        //이전 테스트 데이터가 영향을 주지 않도록 초기화
        inventoryManager.ClearInventory();

        bool breadIngredientResult = inventoryManager.AddItem(breadIngredientId, 2);    //재료 빵 +2

        bool eggIngredientResult = inventoryManager.AddItem(eggIngredientId, 3);        //재료 계란 +3

        bool whiteBreadDishResult = inventoryManager.AddItem(whiteBreadDishId, 1);      //요리 식빵 +1

        //세 아이템의 보유량이 정확한지,
        //서로 다른 ID이므로 슬롯이 총 3개인지 검사
        PrintTestResult(
            "재료와 음식은 각각 별도 슬롯 사용",
            breadIngredientResult &&
            eggIngredientResult &&
            whiteBreadDishResult &&
            inventoryManager.GetItemCount(breadIngredientId) == 2 &&
            inventoryManager.GetItemCount(eggIngredientId) == 3 &&
            inventoryManager.GetItemCount(whiteBreadDishId) == 1 &&
            inventoryManager.SlotCount == 3
            );
    }

    //[현재 게임에서 사용중인 InventoryManager 찾기 메서드]
    private bool TryFindInventoryManager()
    {
        //InventoryManager를 저장하지 않았다면 싱글톤 Instance에서 가져옴
        if (inventoryManager == null)
        {
            inventoryManager = InventoryManager.Instance;
        }

        //InventoryManager를 찾지 못했다면 테스트 씬 설정에 문제가 있다는 오류 출력
        if (inventoryManager == null)
        {
            Debug.LogError(
                "[InventoryManagerTest] InventoryManager를 찾을 수 없습니다. " +
                "씬에 InventoryManager 오브젝트가 있는지 확인하세요."
                );
            return false;   //못 찾았으니 false
        }
        return true;        //찾으면 true
    }

    //[테스트 성공인지 실패인지 출력할 메서드]
    private void PrintTestResult(string testName, bool isSuccess)
    {
        //전달받은 테스트 조건이 성공했는지 확인
        if (isSuccess)
        {
            //성공한 테스트는 일반 로그로 출력
            Debug.Log($"[PASS] {testName}");

            //아래 실패 로그가 실행되지 않도록 종료
            return;
        }

        //실패한 테스트는 빨간색 오류 로그로 출력
        Debug.LogError($"[FAIL] {testName}");
    }
}
