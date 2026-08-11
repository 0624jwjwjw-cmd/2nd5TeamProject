//**InventoryManager의 실제 데이터를 받아 인벤토리 슬롯 UI에 표시하는 Controller**
//
//*역할*
//1. InventoryManager의 슬롯 데이터를 UI로 변환
//2. GameDataRepository에서 아이템 이름 검색
//3. ItemVisualRepository에서 아이콘 검색
//4. InventoryChange 상세 이벤트를 받아 변경된 슬롯만 갱신
//5. 슬롯 선택 상태 관리

using System;
using System.Collections.Generic;
using UnityEngine;

//같은 GameObject에 Controller가 중복으로 붙는 것을 방지
[DisallowMultipleComponent]

public sealed class InventoryUIController : MonoBehaviour, IInitializable
{
    //Inspector 연결
    [Header("Inventory UI")]
    //실제 InventorySlotUI들이 생성될 부모
    //예:
    //InventoryPanel
    //└── Scroll View
    //    └── Viewport
    //        └── Content ← 여기를 연결
    [SerializeField] private Transform slotParent;
    [SerializeField] private InventorySlotUI slotPrefab;    //반복 생성할 InventorySlotUI Prefab

    //Runtime System 참조
    private InventoryManager inventoryManager;              //실제 인벤토리 데이터 관리
    private IGameDataRepository gameDataRepository;         //ID → IngredientData / DishData 검색용(구현체가 아니라 Interface 타입으로 참조)
    private IItemVisualRepository itemVisualRepository;     //ID → Sprite 검색용

    //활성화된 Slot 검색 Dictionary
    //ItemId를 이용해 화면에 존재하는 슬롯을 바로 찾기 위한 Dictionary
    //
    //예:
    //"IG_01"
    //   ↓
    //빵 InventorySlotUI
    //
    //수량 변경 시 모든 UI를 순회하지 않고
    //해당 슬롯을 바로 찾을 수 있음
    private readonly Dictionary<string, InventorySlotUI> slotLookup =
       new Dictionary<string, InventorySlotUI>(StringComparer.Ordinal);

    //선택 상태
    private string selectedItemId = string.Empty;       //현재 선택된 아이템 ID
    public string SelectedItemId => selectedItemId;     //외부 시스템에서 현재 선택 아이템을 확인할 수 있도록 제공
    private bool isInitialized;                         //Controller 초기화 완료 여부

    //Bootstrap 초기화 순서
    //GameDataRepository = -100
    //ItemVisualRepository = -90
    //
    //두 Repository 초기화가 끝난 뒤 UI를 만들기 위해 -80 사용
    public int Priority => -80;

    private void OnEnable()
    {
        //이미 초기화된 Controller가
        //Panel 재활성화 등으로 다시 켜진 경우 이벤트 재구독
        if (isInitialized)
        {
            SubscribeEvents();

            //꺼져 있는 동안 Inventory가 변경됐을 수 있으므로
            //현재 실제 데이터를 기준으로 UI 다시 동기화
            BuildInitialSlots();

            return;
        }

        //다른 씬에서 Repository들이 이미 살아있는 경우
        //BootstrapManager가 없어도 초기화를 시도
        TryInitialize();
    }

    private void Start()
    {
        //OnEnable 시점에는 Repository가 아직 초기화 전일 수 있으므로
        //Start 시점에도 한 번 더 초기화 시도
        if (!isInitialized)
        {
            TryInitialize();
        }
    }

    private void OnDisable()
    {
        //UI가 꺼져 있을 때 불필요하게 이벤트를 받지 않도록 구독 해제
        UnsubscribeEvents();
    }

    //BootstrapManager에서도 호출 가능
    public void Initialize()
    {
        TryInitialize();
    }

    //*초기화*
    private void TryInitialize()
    {
        //이미 초기화했다면 중복 작업 방지
        if (isInitialized) return;

        //Persistent Singleton에서 InventoryManager 가져오기
        inventoryManager = InventoryManager.Instance;

        //GameDataRepository는
        //Interface 타입으로 보관
        gameDataRepository = GameDataRepository.Instance;

        //ItemVisualRepository 역시 Interface 타입으로 보관
        itemVisualRepository = ItemVisualRepository.Instance;

        //InventoryManager가 아직 준비되지 않았다면 대기
        if (inventoryManager == null) return;

        //GameDataRepository가 존재하지 않거나
        //Dictionary 초기화가 아직 끝나지 않았다면 대기
        if (gameDataRepository == null || !gameDataRepository.IsInitialized) return;

        //Visual Repository도 초기화되지 않았다면 대기
        if (itemVisualRepository == null || !itemVisualRepository.IsInitialized) return;

        //Inspector 연결 검사
        if (slotParent == null)
        {
            Debug.LogError("[InventoryUIController] Slot Parent가 연결되지 않았습니다.");
            return;
        }

        if (slotPrefab == null)
        {
            Debug.LogError("[InventoryUIController] Slot Prefab이 연결되지 않았습니다.");
            return;
        }

        //현재 InventoryManager에 이미 들어있는 아이템들을
        //처음 한 번 화면에 생성
        BuildInitialSlots();

        //초기화 완료
        isInitialized = true;

        //현재 UI가 활성화 상태일 때만 이벤트 구독
        if (isActiveAndEnabled)
        {
            SubscribeEvents();
        }

        Debug.Log($"[InventoryUIController] 초기화 완료 | " + $"생성 슬롯: {slotLookup.Count}");
    }

    //*이벤트 구독*
    private void SubscribeEvents()
    {
        if (inventoryManager == null) return;

        //혹시 이전에 같은 이벤트가 등록되어 있었다면 제거
        //중복 호출 방지
        inventoryManager.OnInventoryChangedDetailed -= HandleInventoryChanged;

        //상세 변경 이벤트 등록
        inventoryManager.OnInventoryChangedDetailed += HandleInventoryChanged;
    }

    private void UnsubscribeEvents()
    {
        if (inventoryManager == null) return;

        inventoryManager.OnInventoryChangedDetailed -= HandleInventoryChanged;
    }

    //*최초 Inventory UI 생성*
    private void BuildInitialSlots()
    {
        //Controller가 가지고 있던 UI 슬롯 정리
        ClearAllSlots();

        //현재 InventoryManager의 모든 SlotData 확인
        for (int i = 0; i < inventoryManager.Slots.Count; i++)
        {
            InventorySlotData slotData =
                inventoryManager.Slots[i];

            if (slotData == null || slotData.IsEmpty)
            {
                continue;
            }

            //실제 UI 슬롯 생성
            AddSlot(
                slotData.ItemId,
                slotData.Amount
                );
        }

        //InventoryManager의 데이터 순서와
        //UI 순서를 동일하게 맞춤
        ReorderSlots();
    }

    //*InventoryChange 이벤트 처리*
    private void HandleInventoryChanged(InventoryChange change)
    {
        switch (change.ChangeType)
        {
            //새로운 종류의 아이템이 들어온 경우
            case InventoryChangeType.Added:
                AddSlot(
                    change.ItemId,
                    change.CurrentAmount
                );

                //InventoryManager는 추가 후 ID 정렬을 수행하므로
                //UI 위치도 데이터 순서에 맞춤
                ReorderSlots();
                break;

            //기존 아이템 수량만 바뀐 경우
            case InventoryChangeType.AmountChanged:
                UpdateSlotAmount(
                    change.ItemId,
                    change.CurrentAmount
                );

                break;

            //아이템이 0개가 되어 슬롯 자체가 사라진 경우
            case InventoryChangeType.Removed:
                RemoveSlot(change.ItemId);
                break;

            //정렬 순서만 변경된 경우
            case InventoryChangeType.Sorted:
                ReorderSlots();
                break;

            //인벤토리 전체 초기화
            case InventoryChangeType.Cleared:
                ClearAllSlots();
                break;
        }
    }

    //*Slot 추가*
    private void AddSlot(string itemId, int amount)
    {
        //이미 같은 ID 슬롯이 화면에 존재하면
        //새로 만들지 않고 수량만 갱신
        if (slotLookup.TryGetValue(
            itemId,
            out InventorySlotUI existingSlot))
        {
            existingSlot.UpdateAmount(amount);
            return;
        }

        //아이템 이름과 아이콘 검색
        if (!TryGetDisplayData(
            itemId,
            out string displayName,
            out Sprite icon))
        {
            Debug.LogWarning(
                $"[InventoryUIController] " +
                $"표시할 데이터를 찾지 못했습니다: {itemId}"
            );
            return;
        }

        //Slot Prefab 생성
        InventorySlotUI newSlot =
            Instantiate(
                slotPrefab,
                slotParent
                );

        //Slot UI에 실제 표시 데이터 전달
        newSlot.Setup(
            itemId,
            icon,
            displayName,
            amount,
            HandleSlotClicked
            );

        //ID → UI Slot 연결
        slotLookup.Add(
            itemId,
            newSlot
            );

        //현재 선택된 아이템과 같은 ID라면
        //선택 테두리 복구
        newSlot.SetSelected(
            string.Equals(
                selectedItemId,
                itemId,
                StringComparison.Ordinal
                )
            );
    }

    //*수량만 갱신*
    private void UpdateSlotAmount(string itemId, int amount)
    {
        //Dictionary에서 해당 아이템 UI를 바로 검색
        if (slotLookup.TryGetValue(
            itemId,
            out InventorySlotUI slot))
        {
            //전체 Setup을 다시 하지 않고
            //수량 Text만 변경
            slot.UpdateAmount(amount);
            return;
        }

        //데이터에는 존재하는데
        //UI 슬롯이 없는 예외 상황 방어
        AddSlot(itemId, amount);

        ReorderSlots();
    }

    //*Slot 제거*
    private void RemoveSlot(string itemId)
    {
        //해당 UI 슬롯 검색
        if (!slotLookup.TryGetValue(itemId, out InventorySlotUI slot)) return;

        //현재 선택 중이던 아이템이 삭제된 경우 선택 ID도 비움
        if (string.Equals(selectedItemId, itemId, StringComparison.Ordinal)) selectedItemId = string.Empty;

        //Dictionary에서 제거
        slotLookup.Remove(itemId);

        //현재 단계에서는 UI 연결 확인이 먼저이므로 Destroy 사용
        //
        //다음 최적화 단계에서
        //이 부분을 Slot Pool 반환으로 교체할 예정
        Destroy(slot.gameObject);
    }

    //*전체 Slot 정리*
    private void ClearAllSlots()
    {
        //Controller가 생성한 Slot들만 제거
        foreach (KeyValuePair<string, InventorySlotUI> pair in slotLookup)
        {
            InventorySlotUI slot = pair.Value;

            if (slot != null)
            {
                Destroy(slot.gameObject);
            }
        }

        //Dictionary 초기화
        slotLookup.Clear();

        //선택 상태도 초기화
        selectedItemId = string.Empty;
    }

    //*UI 순서 맞추기*
    private void ReorderSlots()
    {
        //InventoryManager의 정렬된 실제 데이터 순서를 기준으로
        //UI의 Sibling 순서도 변경
        for (int i = 0; i < inventoryManager.Slots.Count; i++)
        {
            InventorySlotData slotData =
                inventoryManager.Slots[i];

            if (slotData == null) continue;

            if (slotLookup.TryGetValue(
                slotData.ItemId,
                out InventorySlotUI slot))
            {
                //Hierarchy상 순서를 데이터 List와 동일하게 맞춤
                slot.transform.SetSiblingIndex(i);
            }
        }
    }

    //*ID → UI 표시 정보 검색*
    private bool TryGetDisplayData(string itemId, out string displayName, out Sprite icon)
    {
        //기본값
        displayName = string.Empty;
        icon = null;

        //재료 검색
        if (gameDataRepository.TryGetIngredient(itemId, out IngredientData ingredientData))
        {
            displayName = ingredientData.IngredientName;
        }

        //일반 요리 검색
        else if (gameDataRepository.TryGetDish(itemId, out DishData dishData))
        {
            displayName = dishData.DishName;
        }

        //특별 요리 검색
        else if (gameDataRepository.TryGetSpecialDish(itemId, out DishData specialDishData))
        {
            displayName = specialDishData.DishName;
        }

        //어느 데이터 Repository에서도 ID를 찾지 못했다면 실패
        else return false;

        itemVisualRepository.TryGetIcon(itemId, out icon);

        return true;
    }

    //*Slot 클릭*
    private void HandleSlotClicked(string itemId)
    {
        //이전에 선택했던 Slot이 존재한다면
        //선택 테두리 제거
        if (!string.IsNullOrWhiteSpace(selectedItemId) &&
            slotLookup.TryGetValue(
                selectedItemId,
                out InventorySlotUI previousSlot))
        {
            previousSlot.SetSelected(false);
        }

        //새롭게 선택한 아이템 ID 저장
        selectedItemId = itemId;

        //새 Slot 선택 테두리 표시
        if (slotLookup.TryGetValue(
            selectedItemId,
            out InventorySlotUI selectedSlot))
        {
            selectedSlot.SetSelected(true);
        }

        Debug.Log(
            $"[InventoryUIController] 아이템 선택: " +
            $"{selectedItemId}"
        );
    }
}