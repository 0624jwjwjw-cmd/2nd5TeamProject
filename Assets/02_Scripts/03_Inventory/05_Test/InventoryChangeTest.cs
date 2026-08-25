//**InventoryManager의 상세 변경 이벤트가
//Added / AmountChanged / Removed / Sorted / Cleared를
//정확하게 전달하는지 테스트하는 스크립트**
using System;
using UnityEngine;

//같은 GameObject에 테스트 컴포넌트가 중복으로 붙는 것 방지
[DisallowMultipleComponent]

public sealed class InventoryChangeTest : MonoBehaviour
{
    //Inspector 연결
    [Header("테스트할 Inventory Manager")]
    [SerializeField] private InventoryManager inventoryManager;

    //이벤트 테스트용 변수
    private InventoryChange lastChange; //가장 마지막으로 전달받은 상세 변경 정보
    private int detailedEventCount;     //한 번의 인벤토리 변경에서 상세 이벤트가 몇 번 발생했는지 확인
    private int simpleEventCount;       //기존 OnInventoryChanged 이벤트도 정상적으로 한 번 발생하는지 확인

    //*전체 테스트*
    [ContextMenu("Inventory Change 전체 테스트")]
    public void RunAllTests()
    {
        //Edit Mode에서 인벤토리 데이터를 수정하지 않도록 방지
        if (!Application.isPlaying)
        {
            Debug.LogError("[InventoryChangeTest] Play 모드에서 테스트해주세요.");
            return;
        }

        //InventoryManager가 연결되지 않았다면 테스트 불가능
        if (inventoryManager == null)
        {
            Debug.LogError("[InventoryChangeTest] InventoryManager가 연결되지 않았습니다.");
            return;
        }

        //테스트 시작 전 인벤토리 초기화
        //기존 테스트 데이터가 남아 있으면
        //PreviousAmount 등의 결과가 달라질 수 있으므로 먼저 비움
        //
        //이 시점에는 아직 이벤트를 구독하지 않았으므로
        //Clear 이벤트는 테스트 결과에 포함되지 않음
        inventoryManager.ClearInventory();

        //이벤트 구독
        //새 상세 이벤트 구독
        inventoryManager.OnInventoryChangedDetailed += HandleDetailedChange;

        //기존 이벤트도 계속 정상 동작하는지 확인하기 위해 구독
        inventoryManager.OnInventoryChanged += HandleSimpleChange;

        //성공한 테스트 개수
        int passedCount = 0;

        //전체 테스트 개수
        int totalCount = 0;

        try
        {
            //Test 1.
            //새로운 아이템 추가
            totalCount++;

            if (RunTestCase(
                "새 아이템 추가",
                () => inventoryManager.AddItem("IG_01", 3, ItemType.Ingredient),
                InventoryChangeType.Added,
                "IG_01",
                0,
                3))
            {
                passedCount++;
            }

            //Test 2.
            //기존 아이템 수량 증가
            totalCount++;

            if (RunTestCase(
                "기존 아이템 수량 증가",
                () => inventoryManager.AddItem("IG_01", 2, ItemType.Ingredient),
                InventoryChangeType.AmountChanged,
                "IG_01",
                3,
                5))
            {
                passedCount++;
            }

            //TEST 3.
            //아이템 일부 제거
            totalCount++;

            if (RunTestCase(
                "아이템 일부 제거",
                () => inventoryManager.RemoveItem("IG_01", 3),
                InventoryChangeType.AmountChanged,
                "IG_01",
                5,
                2))
            {
                passedCount++;
            }

            //TEST 4.
            //아이템 전부 제거
            totalCount++;

            if (RunTestCase(
                "아이템 전부 제거",
                () => inventoryManager.RemoveItem("IG_01", 2),
                InventoryChangeType.Removed,
                "IG_01",
                2,
                0))
            {
                passedCount++;
            }

            //TEST 5.
            //재료 추가
            totalCount++;

            if (RunTestCase(
                "재료 추가",
                () => inventoryManager.AddItem("IG_04", 1, ItemType.Ingredient),
                InventoryChangeType.Added,
                "IG_04",
                0,
                1))
            {
                passedCount++;
            }

            //Test 6.
            //요리 추가
            totalCount++;

            if (RunTestCase(
                "요리 추가",
                () => inventoryManager.AddItem("DS_01", 1, ItemType.Dish),
                InventoryChangeType.Added,
                "DS_01",
                0,
                1))
            {
                passedCount++;
            }

            //TEST 7.
            //정렬
            totalCount++;

            if (RunTestCase(
                "인벤토리 정렬",
                () => inventoryManager.SortByItemId(),
                InventoryChangeType.Sorted,
                string.Empty,
                0,
                0))
            {
                passedCount++;
            }

            //TEST 8.
            //전체 초기화
            totalCount++;

            if (RunTestCase(
                "인벤토리 전체 초기화",
                () => inventoryManager.ClearInventory(),
                InventoryChangeType.Cleared,
                string.Empty,
                0,
                0))
            {
                passedCount++;
            }
        }
        finally
        {
            //이벤트 구독 해제
            //테스트를 여러 번 실행해도 이벤트가
            //중복 등록되지 않도록 반드시 해제
            inventoryManager.OnInventoryChangedDetailed -= HandleDetailedChange;

            inventoryManager.OnInventoryChanged -= HandleSimpleChange;
        }

        //최종 결과
        if (passedCount == totalCount)
        {
            Debug.Log(
                $"[InventoryChangeTest] TEST PASS " +
                $"({passedCount}/{totalCount})"
                );
        }
        else
        {
            Debug.LogError(
                $"[InventoryChangeTest] TEST FAIL " +
                $"({passedCount}/{totalCount})"
                );
        }
    }

    //*개별 테스트 실행*
    private bool RunTestCase(
        string testName,
        Action operation,
        InventoryChangeType expectedType,
        string expectedItemId,
        int expectedPreviousAmount,
        int expectedCurrentAmount)
    {
        //이번 테스트 전에 이벤트 기록 초기화
        detailedEventCount = 0;
        simpleEventCount = 0;
        lastChange = default;

        //실제 InventoryManager 기능 실행
        operation.Invoke();

        //이벤트 발생 횟수 확인
        //상세 이벤트는 변경 한 번당 정확히 한 번이어야 함
        if (detailedEventCount != 1)
        {
            Debug.LogError(
                $"[InventoryChangeTest] {testName} 실패 | " +
                $"상세 이벤트 발생 횟수: {detailedEventCount}"
                );

            return false;
        }

        //기존 단순 이벤트 역시 한 번 발생해야 함
        if (simpleEventCount != 1)
        {
            Debug.LogError(
                $"[InventoryChangeTest] {testName} 실패 | " +
                $"기존 이벤트 발생 횟수: {simpleEventCount}"
                );

            return false;
        }

        //상세 데이터 비교
        bool typeMatches = lastChange.ChangeType == expectedType;

        bool idMatches =
            string.Equals(
                lastChange.ItemId,
                expectedItemId,
                StringComparison.Ordinal
                );

        bool previousMatches = lastChange.PreviousAmount == expectedPreviousAmount;

        bool currentMatches = lastChange.CurrentAmount == expectedCurrentAmount;

        //하나라도 예상 결과와 다르면 실패
        if (!typeMatches ||
            !idMatches ||
            !previousMatches ||
            !currentMatches)
        {
            Debug.LogError(
                $"[InventoryChangeTest] {testName} 실패\n" +

                $"예상값 → " +
                $"{expectedType}, " +
                $"{expectedItemId}, " +
                $"{expectedPreviousAmount} → {expectedCurrentAmount}\n" +

                $"실제값 → " +
                $"{lastChange.ChangeType}, " +
                $"{lastChange.ItemId}, " +
                $"{lastChange.PreviousAmount} → {lastChange.CurrentAmount}"
                );

            return false;
        }

        //모든 값이 정확하면 성공
        Debug.Log(
            $"[InventoryChangeTest] {testName} 성공 | " +
            $"{lastChange.ChangeType} | " +
            $"{lastChange.ItemId} | " +
            $"{lastChange.PreviousAmount} → {lastChange.CurrentAmount}"
            );

        return true;
    }

    //*상세 이벤트 수신*
    private void HandleDetailedChange(InventoryChange change)
    {
        lastChange = change;        //전달받은 변경 정보를 저장

        detailedEventCount++;       //상세 이벤트 발생 횟수 증가
    }

    //*기존 이벤트 수신*
    private void HandleSimpleChange()
    {
        simpleEventCount++;         //기존 이벤트 발생 횟수 증가
    }
}
