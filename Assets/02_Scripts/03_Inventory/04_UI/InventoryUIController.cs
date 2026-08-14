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
    [SerializeField] private InventorySlotPool slotPool;    //InventorySlotUI의 생성과 재사용을 담당하는 전용 Pool

    [Header("Category Selected Visual")]
    [SerializeField] private GameObject allSelectedFrame;          //전체 탭 선택 표시
    [SerializeField] private GameObject ingredientSelectedFrame;   //재료 탭 선택 표시
    [SerializeField] private GameObject dishSelectedFrame;         //요리 탭 선택 표시

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

    //현재 인벤토리에서 선택되어 있는 필터 종류
    //처음 인벤토리를 열었을 때 모든 아이템이 보이도록 All을 기본값으로 설정
    private InventoryCategory currentCategory = InventoryCategory.All;

    //현재 선택된 인벤토리 카테고리를 외부에서 확인하기 위한 읽기 전용 프로퍼티
    public InventoryCategory CurrentCategory => currentCategory;

    private bool isInitialized;                         //Controller 초기화 완료 여부

    //Bootstrap 초기화 순서
    //GameDataRepository = -100
    //ItemVisualRepository = -90
    //
    //두 Repository 초기화가 끝난 뒤 UI를 만들기 위해 -80 사용
    public int Priority => -80;

    private void OnEnable()
    {
        //이미 시스템 초기화가 끝난 상태라면
        //인벤토리 창이 실제로 열리는 순간 현재 데이터를 UI로 생성
        if (isInitialized)
        {
            //꺼져 있는 동안 Inventory가 변경됐을 수 있으므로
            //현재 실제 데이터를 기준으로 UI 다시 동기화
            BuildInitialSlots();

            //창이 열려 있는 동안만 Inventory 변경 이벤트 수신
            SubscribeEvents();

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

    //*인벤토리 카테고리 변경*
    public void SetCategory(InventoryCategory category)
    {
        //이미 현재 보고 있는 카테고리라면 UI를 다시 생성하지 않고 선택 표시만 현재 상태와 맞춤
        if (currentCategory == category)
        {
            UpdateCategoryVisual();
            return;
        }

        currentCategory = category; //현재 카테고리 변경

        //현재 선택된 카테고리에 맞춰
        //버튼의 SelectedFrame 상태를 갱신
        UpdateCategoryVisual();

        //아직 초기화 전이거나
        //현재 UI가 비활성화 상태라면 값만 저장
        if (!isInitialized || !isActiveAndEnabled) return;

        //카테고리가 변경되면 현재 인벤토리 데이터를 기준으로 슬롯 목록을 다시 구성
        BuildInitialSlots();
    }

    //*전체 아이템 표시*
    public void ShowAll()
    {
        SetCategory(InventoryCategory.All);
    }

    //*재료만 표시*
    public void ShowIngredients()
    {
        SetCategory(InventoryCategory.Ingredient);
    }

    //*요리만 표시*
    public void ShowDishes()
    {
        SetCategory(InventoryCategory.Dish);
    }

    //*현재 선택된 카테고리에 맞춰 버튼의 선택 표시 갱신*
    private void UpdateCategoryVisual()
    {
        //현재 카테고리가 All이면
        //전체 버튼의 SelectedFrame만 활성화
        if (allSelectedFrame != null)
        {
            allSelectedFrame.SetActive(
                currentCategory == InventoryCategory.All
            );
        }

        //현재 카테고리가 Ingredient이면
        //재료 버튼의 SelectedFrame만 활성화
        if (ingredientSelectedFrame != null)
        {
            ingredientSelectedFrame.SetActive(
                currentCategory == InventoryCategory.Ingredient
            );
        }

        //현재 카테고리가 Dish이면
        //요리 버튼의 SelectedFrame만 활성화
        if (dishSelectedFrame != null)
        {
            dishSelectedFrame.SetActive(
                currentCategory == InventoryCategory.Dish
            );
        }
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

        //InventorySlotPool이 연결 검사
        if (slotPool == null)
        {
            Debug.LogError("[InventoryUIController] Slot Pool이 연결되지 않았습니다.");
            return;
        }

        //초기화 완료
        isInitialized = true;

        //현재 기본 카테고리에 맞춰
        //카테고리 버튼의 선택 표시를 초기화
        UpdateCategoryVisual();

        //현재 UI가 활성화 상태일 때만
        //Inventory 슬롯을 생성하고 변경 이벤트를 구독
        if (isActiveAndEnabled)
        {
            //현재 실제 Inventory 데이터를 기준으로 UI 생성
            BuildInitialSlots();

            //인벤토리 창이 열려있는 동안만 변경 이벤트 수신
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
        //현재 선택된 카테고리에서 보여주지 않은 아이템이라면 Slot을 생성하지 않음
        if (!IsVisibleItem(itemId)) return;

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
            out Sprite icon,
            out bool isSpecial))
        {
            Debug.LogWarning(
                $"[InventoryUIController] " +
                $"표시할 데이터를 찾지 못했습니다: {itemId}"
            );
            return;
        }

        //Pool에서 사용할 InventorySlotUI를 가져옴
        InventorySlotUI newSlot = slotPool.GetSlot(slotParent);

        //Pool에서 Slot을 가져오지 못한 예외 상황이라면
        //이후 Setup을 진행할 수 없으므로 종료
        if (newSlot == null)
        {
            Debug.LogError("[InventoryUIController] InventorySlotPool에서 Slot을 가져오지 못했습니다.");
            return;
        }

        //Slot UI에 실제 표시 데이터 전달
        newSlot.Setup(
            itemId,
            icon,
            displayName,
            amount,
            HandleSlotClicked
            );

        //특별 요리라면 S 배지를 표시
        newSlot.SetSpecialBadge(isSpecial);

        //ID → UI Slot 연결
        slotLookup.Add(itemId, newSlot);

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
        //현재 선택된 카테고리에서 보여주지 않은 아이템이라면 Slot을 생성하지 않음
        if (!IsVisibleItem(itemId)) return;

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

        //화면에서 더 이상 사용하지 않는 Slot을 InventorySlotPool에 반환
        //
        //반환된 Slot은 비활성화 상태로 보관되며
        //다음 Slot 생성 요청 때 다시 재사용됨
        slotPool.ReleaseSlot(slot);
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
                //화면에서 사용 중이던 Slot을 삭제하지 않고
                //InventorySlotPool에 반환해서 다음에 재사용
                slotPool.ReleaseSlot(slot);
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
        //현재 실제 화면에 표시되는 슬롯의 순서
        int visibleIndex = 0;

        //InventoryManager의 정렬된 실제 데이터 순서를 기준으로
        //UI의 Sibling 순서도 변경
        for (int i = 0; i < inventoryManager.Slots.Count; i++)
        {
            InventorySlotData slotData =
                inventoryManager.Slots[i];

            if (slotData == null) continue;

            //현재 화면에 실제 생성되어 있는 Slot만 순서 변경
            if (slotLookup.TryGetValue(
                slotData.ItemId,
                out InventorySlotUI slot))
            {
                //필터로 숨겨진 아이템은 제외하고
                //보이는 슬롯끼리 0, 1, 2... 순서로 배치
                slot.transform.SetSiblingIndex(visibleIndex);

                visibleIndex++;
            }
        }
    }

    //*ID → UI 표시 정보 검색*
    private bool TryGetDisplayData(string itemId, out string displayName, out Sprite icon, out bool isSpecial)
    {
        //기본값
        displayName = string.Empty;
        icon = null;
        isSpecial = false;

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

            //Repository에서 특별 요리로 조회된 경우에만
            //S 배지를 표시하도록 true 설정
            isSpecial = true;
        }

        //어느 데이터 Repository에서도 ID를 찾지 못했다면 실패
        else return false;

        itemVisualRepository.TryGetIcon(itemId, out icon);

        return true;
    }

    //*현재 선택된 인벤토리 카테고리에서
    //전달받은 아이템을 보여줘야하는지 확인하는 메서드*
    //
    //true: 현재 탭에서 보여줄 아이템 / false: 현재 탭에서는 숨길 아이템
    private bool IsVisibleItem(string itemId)
    {
        //아이템 ID가 비어있으면
        //어떤 종류의 아이템인지 확인할 수 없으므로 표시하지 않음
        if (string.IsNullOrWhiteSpace(itemId)) return false;

        //현재 선택된 인벤토리 카테고리에 따라
        //아이템을 표시할지 결정
        switch (currentCategory)
        {
            //==================================================
            //전체 탭
            //==================================================
            case InventoryCategory.All:

                //전체 탭에서는
                //인벤토리에 존재하는 모든 아이템을 표시
                return true;


            //==================================================
            //재료 탭
            //==================================================
            case InventoryCategory.Ingredient:

                //GameDataRepository에서
                //해당 ItemId를 IngredientData로 찾을 수 있다면
                //재료 아이템이므로 표시
                return gameDataRepository.TryGetIngredient(
                    itemId,
                    out _
                );


            //==================================================
            //요리 탭
            //==================================================
            case InventoryCategory.Dish:

                //일반 요리 Repository에서
                //해당 ItemId를 찾을 수 있다면 표시
                if (gameDataRepository.TryGetDish(
                    itemId,
                    out _))
                {
                    return true;
                }


                //일반 요리가 아니라면
                //특별 요리 Repository에서 다시 검색
                //
                //특별 요리도 Dish 탭에 함께 표시하기 때문에
                //찾았다면 true 반환
                return gameDataRepository.TryGetSpecialDish(
                    itemId,
                    out _
                );


            //정의되지 않은 카테고리 값이 들어온 경우
            //안전하게 표시하지 않음
            default:
                return false;
        }
    }

    //*Slot 클릭*
    private void HandleSlotClicked(string itemId)
    {
        //이미 선택된 슬롯을 다시 터치한 경우
        //현재 선택된 아이템과 방금 터치한 아이템이 같은지 확인
        bool isSameSelectedItem = string.Equals(selectedItemId, itemId, StringComparison.Ordinal);

        if (isSameSelectedItem)
        {
            //현재 선택된 슬롯 검색
            if (slotLookup.TryGetValue(itemId, out InventorySlotUI sameSlot))
            {
                sameSlot.SetSelected(false);    //선택 테두리 OFF
            }

            //선택된 아이템이 없는 상태로 변경
            selectedItemId = string.Empty;

            Debug.Log("[InventoryUIController] 아이템 선택 취소");

            return;
        }

        //다른 슬롯을 새로 터치한 경우
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