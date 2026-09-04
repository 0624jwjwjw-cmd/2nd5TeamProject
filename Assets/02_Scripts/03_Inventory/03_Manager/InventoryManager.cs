//**인벤토리 매니저**
using System;
using System.Collections.Generic;   //List사용
using UnityEngine;

//하나의 게임 오브젝트에 특정 컴포넌트를 오직 하나만 추가할 수 있게 제한하는 어트리뷰트
[DisallowMultipleComponent]

public class InventoryManager : MonoBehaviour,ISaveable
{
    //현재 게임에서 사용 중인 InventoryManager를
    //다른 스크립트가 쉽게 접근 가능하도록 제공하는 싱글톤 프로퍼티
    public static InventoryManager Instance { get; private set; }

    [Header("인벤토리 설정")]
    [Min(1)][SerializeField] private int maxStackSize = 100; //슬롯당 1 ~ 100로 기본값 설정

    [Header("현재 인벤토리 데이터")]
    //InventorySlotData를 여러 개 보관하는 실제 인벤토리 목록
    [SerializeField] private List<InventorySlotData> slots = new List<InventorySlotData>();

    //인벤토리 내용이 변경되었을 때 실행되는 이벤트
    //InventoryUIController가 이 이벤트를 구독하면 아이템 추가ㆍ제거 후 UI를 자동으로 갱신 가능
    //팀원이 사용해도 깨지지 않게 기존 이벤트 일단 안지울게요
    public event Action OnInventoryChanged;

    //인벤토리가 구체적으로 어떻게 변경됐는지 전달하는 상세 이벤트
    //InventoryUIController가 변경된 슬롯만 갱신할 때 사용
    public event Action<InventoryChange> OnInventoryChangedDetailed;

    //프로퍼티
    //IReadOnlyList로 제공하기 때문에
    public IReadOnlyList<InventorySlotData> Slots => slots; //slots.Add()나 slots.Remove()직접 호출 불가

    //ItemId로 현재 인벤토리에 있는 슬롯 데이터를 찾음
    public bool TryGetSlot(
        string itemId,
        out InventorySlotData slotData)
    {
        //찾기 실패 기본값
        slotData = null;

        //빈 ID는 찾을 수 없음
        if (string.IsNullOrWhiteSpace(itemId)) return false;
        
        //InventoryManager 내부의 슬롯 검색 기능 사용
        slotData = FindSlot(itemId);

        //슬롯을 실제로 찾았는지 반환
        return slotData != null;
    }

    //*외부 시스템에서 최대 스택 수량을 확인하거나 변경*
    public int MaxStackSize
    {
        get => maxStackSize;
        set => maxStackSize = Mathf.Max(1, value);
    }

    public int SlotCount => slots.Count;                    //슬롯 개수 외부에 제공

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;                //현재 InventoryManager를 싱글톤 인스턴스로 저장
            DontDestroyOnLoad(gameObject);  //씬이 변경되어도 현재 게임 오브젝트가 삭제되지 않고 유지
            PrepareInventoryData();         //Inspector 또는 저장 데이터에서 들어온 슬롯 목록을 사용 가능한 상태로 정리
            return;                         //현재 객체 등록이 끝났으므로 메서드 종료
        }

        if (Instance != this)
        {
            //중복 InventoryManager가 붙은 게임 오브젝트 제거
            Destroy(gameObject);
        }
    }

    //*Unity Inspector에서 값이 변경될 때 호출되는 메서드*
    //게임을 실행하지 않은 상태에서도
    //잘못된 설정값이 들어가지 않도록 보정
    private void OnValidate()
    {
        //최대 스택 수량이 1보다 작아지지 않도록 제한
        maxStackSize = Mathf.Max(1, maxStackSize);
    }

    //*Inspector나 저장 데이터로 들어온 인벤토리 목록을 실제 게임에서 사용하기 전에 정리하는 메서드*
    private void PrepareInventoryData()
    {
        if (slots == null)
        {
            //slots 리스트가 null이면 새로운 빈 리스트 생성
            slots = new List<InventorySlotData>();
        }

        //리스트 안에 null 슬롯이나 빈 슬롯이 있다면 제거
        slots.RemoveAll(slot => slot == null || slot.IsEmpty);

        //게임 시작 시
        //재료 → 일반 요리 → 특별 요리 순서로 정렬하고,
        //같은 종류 안에서는 ItemId 숫자 순서로 정렬
        SortSlotsByItemTypeAndId();
    }

    //*아이템을 인벤토리에 추가하는 메서드*
    //상점에서 재료를 구매하거나, 요리 시스템에서 완성된 음식을 지급할 때 사용
    public bool AddItem(string itemId, int amount, ItemType itemType)
    {
        //전달받은 아이템 ID가 비어 있는지 검사
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning("[InventoryManager] 아이템 ID가 비어 있어 추가할 수 없습니다.");
            //추가에 실패했으므로 false반환
            return false;
        }

        //아이템 ID 접두사와 전달받은 ItemType이
        //서로 일치하는지 검사
        if (!IsItemTypeMatchingId(itemId, itemType))
        {
            Debug.LogWarning(
                $"[InventoryManager] ID와 ItemType이 일치하지 않습니다. " +
                $"ID: {itemId}, ItemType: {itemType}"
                );

            //잘못된 타입으로 슬롯이 생성되는 것을 방지
            return false;
        }

        //추가하려는 수량이 1개 미만인지 검사
        if (amount <= 0)
        {
            Debug.LogWarning($"[InventoryManager] 추가 수량은 1 이상이어야 합니다. 입력값: {amount}");
            //추가에 실패했으므로 false반환
            return false;
        }

        //현재 인벤토리에 해당 수량을 추가할 수 있는지 검사
        if (!CanAddItem(itemId, amount))
        {
            Debug.LogWarning(
                $"[InventoryManager] {itemId} 아이템을 {amount}개 추가할 수 없습니다. " +
                $"슬롯당 최대 수량: {maxStackSize}"
                );
            //추가에 실패했으므로 false반환
            return false;
        }

        //현재 인벤토리에서 동일한 아이템 슬롯 찾기
        InventorySlotData existingSlot = FindSlot(itemId);

        //이번 변경 내용을 저장할 변수
        InventoryChange change;

        //동일한 아이템 슬롯이 이미 존재하면
        if (existingSlot != null)
        {
            //변경되기 전 수량 저장
            int previousAmount = existingSlot.Amount;

            //기존 슬롯의 수량 증가
            existingSlot.AddAmount(amount);

            //수량만 변경됐다는 상세 정보 생성
            change = new InventoryChange(
                InventoryChangeType.AmountChanged,
                itemId, 
                previousAmount, 
                existingSlot.Amount
                );
        }
        //처음 들어오는 아이템
        else
        {
            //전달받은 아이템 정보로 새로운 슬롯 데이터를 생성
            InventorySlotData newSlot =
                new InventorySlotData(
                    itemId,
                    amount,
                    itemType
                    );

            //새로 만든 슬롯을 전체 인벤토리 목록에 추가
            slots.Add(newSlot);

            //기존에는 존재하지 않았으므로 PreviousAmount는 0
            change = new InventoryChange(
                InventoryChangeType.Added,
                itemId,
                0,
                amount
                );
        }

        //아이템이 추가된 뒤
        //재료 → 일반 요리 → 특별 요리 순서로 다시 정렬하고,
        //같은 종류 안에서는 ItemId 숫자 순서로 정렬
        SortSlotsByItemTypeAndId();

        //기존 이벤트 + 상세 이벤트 발생
        NotifyInventoryChanged(change);

        //아이템 추가에 성공했으므로 true 반환
        return true;
    }

    //*해당 아이템을 지정한 수량만큼 추가할 수 있는지 검사하는 메서드*
    //ShopManager가 후원금을 먼저 차감하기 전에 인벤토리에 공간이 있는지 확인할 때 사용
    public bool CanAddItem(string itemId, int amount)
    {
        //아이템 ID가 비어 있으면 추가할 수 없으므로 false
        if (string.IsNullOrWhiteSpace(itemId)) return false;
        //수량이 1개 미만이면 정상적인 추가 요청이 아니므로 false
        if (amount <= 0) return false;

        //현재 인벤토리에서 같은 아이템 슬롯 찾기
        InventorySlotData existingSlot = FindSlot(itemId);

        //같은 아이템 슬롯이 없다면 새 슬롯에 전달받은 수량을 넣어야 함
        if (existingSlot == null)
        {
            //추가 수량이 최대 스택 수량 이하인지 반환
            return amount <= maxStackSize;
        }

        //기존 수량과 추가 수량의 합이 최대 스택 수량 이하인지 검사해서 결과 반환
        return existingSlot.Amount + amount <= maxStackSize;
    }

    //*아이템을 지정한 수량만큼 제거하는 메서드*
    //요리할 때 재료를 소비하거나 라이브에서 음식을 먹었을 때 사용
    public bool RemoveItem(string itemId, int amount)
    {
        //전달받은 아이템 ID가 비어 있는지 검사
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning("[InventoryManager] 아이템 ID가 비어 있어 제거할 수 없습니다.");

            //제거에 실패했으므로 false반환
            return false;
        }

        //제거하려는 수량이 1개 미만인지 검사
        if (amount <= 0)
        {
            Debug.LogWarning($"[InventoryManager] 제거 수량은 1 이상이어야 합니다. 입력값: {amount}");

            //제거에 실패했으므로 false반환
            return false;
        }

        //현재 인벤토리에서 제거할 아이템 슬롯 찾기
        InventorySlotData targetSlot = FindSlot(itemId);

        //해당 아이템 슬롯이 존재하지 않는지 검사
        if (targetSlot == null)
        {
            Debug.LogWarning($"[InventoryManager] 보유하지 않은 아이템입니다. 아이템 ID: {itemId}");

            //제거에 실패했으므로 false반환
            return false;
        }

        //아이템을 제거하기 전에 기존 수량 저장
        //수량이 0이 되면 InventorySlotData.Clear()가 실행되므로
        //변경 전 값을 먼저 기억해둬야 함
        int previousAmount = targetSlot.Amount;

        //InventorySlotData에 지정한 수량 제거를 요청
        bool removeSucceeded = targetSlot.TryRemoveAmount(amount);

        //보유 수량이 부족해서 제거에 실패했는지 검사
        if (!removeSucceeded)
        {
            Debug.LogWarning($"[InventoryManager] {itemId} 아이템의 수량이 부족합니다. " +
                $"보유 수량: {targetSlot.Amount}, 요청 수량: {amount}");

            //제거에 실패했으므로 false반환
            return false;
        }

        //수량이 0이 되어 슬롯 자체가 사라진 경우
        if (targetSlot.IsEmpty)
        {
            //수량이 0이 된 슬롯을 인벤토리 목록에서 제거
            slots.Remove(targetSlot);

            //Remove 이벤트 생성
            InventoryChange change = new InventoryChange(
                InventoryChangeType.Removed,
                itemId,
                previousAmount,
                0
                );

            //인벤토리가 변경되었다는 이벤트 발생시키기
            NotifyInventoryChanged(change);
        }
        //아이템은 남아있고 수량만 줄어든 경우
        else
        {
            InventoryChange change = new InventoryChange(
                InventoryChangeType.AmountChanged,
                itemId,
                previousAmount,
                targetSlot.Amount
                );
            NotifyInventoryChanged(change);
        }

        //제거 성공했으므로 true 반환
        return true;
    }

    //*특정 아이템을 필요한 수량만큼 보유하고 있는지 검사하는 메서드*
    //요리 시작 전에 필요한 재료가 있는지 확인하거나
    //라이브 시작 전에 음식이 있는지 확인할 때 사용
    public bool HasItem(string itemId, int requireAmount)
    {
        //아이템 ID가 비어 있다면 보유 여부를 검사할 수 없으므로 false
        if (string.IsNullOrWhiteSpace(itemId)) return false;

        //필요한 수량이 1개 미만이면 정상적인 검사 요청이 아니므로 false
        if (requireAmount <= 0) return false;

        //현재 보유 수량이 필요한 수량 이상인지 검사해서 반환
        return GetItemCount(itemId) >= requireAmount;
    }

    //*특정 아이템을 현재 몇 개 보유하고 있는지 반환하는 메서드*
    public int GetItemCount(string itemId)
    {
        //아이템 ID가 비어 있다면 해당 아이템을 찾을 수 없으므로 0 반환
        if (string.IsNullOrWhiteSpace(itemId)) return 0;

        //현재 인벤토리에서 해당 아이템 슬롯 찾기
        InventorySlotData targetSlot = FindSlot(itemId);

        //슬롯을 찾지 못했다면 보유하지 않은 것이므로 0 반환
        if (targetSlot == null) return 0;

        //슬롯을 찾았다면 현재 보유 수량 반환
        return targetSlot.Amount;
    }

    //*현재 인벤토리에 들어 있는 모든 아이템을 제거하는 메서드*
    //데이터 초기화 또는 디버그 사용
    public void ClearInventory()
    {
        //이미 인벤토리가 비어 있으면 메서드 종료
        if (slots.Count == 0) return;

        //모든 슬롯 데이터를 리스트에서 제거
        slots.Clear();

        InventoryChange change = new InventoryChange(
            InventoryChangeType.Cleared,
            string.Empty,
            0,
            0
            );

        //인벤토리가 변경되었다는 이벤트 발생
        NotifyInventoryChanged(change);
    }

    //*아이템 ID 접두사와 ItemType이 일치하는지 검사하는 메서드*
    //
    //현재 프로젝트 아이템 ID 규칙
    //IG_ = 재료
    //DS_ = 일반 요리
    //TD_ = 음식물 쓰레기
    //BD_ = 탄 음식
    //SD_ = 특별 요리
    private bool IsItemTypeMatchingId(string itemId, ItemType itemType)
    {
        //아이템 ID가 비어 있다면 타입을 확인할 수 없음
        if (string.IsNullOrWhiteSpace(itemId)) return false;
        
        //IG_로 시작하는 ID는 Ingredient 타입이어야 함
        if (itemId.StartsWith("IG_", StringComparison.Ordinal))
        {
            return itemType == ItemType.Ingredient;
        }

        //DS_는 정상적으로 완성된 일반 요리 ID
        //TD_는 잘못된 재료 조합으로 만들어진 음식물 쓰레기 ID
        //BD_는 조리 과정에서 만들어진 탄 음식 ID
        //
        //세 데이터는 서로 다른 ID 접두사를 사용하지만
        //모두 DishData를 사용하는 음식이므로
        //인벤토리에서는 ItemType.Dish로 동일하게 처리
        if (itemId.StartsWith("DS_", StringComparison.Ordinal) || 
            itemId.StartsWith("TD_", StringComparison.Ordinal) ||
            itemId.StartsWith("BD_", StringComparison.Ordinal))
        {
            return itemType == ItemType.Dish;
        }

        //SD_로 시작하는 ID는 SpecialDish 타입이어야 함
        if (itemId.StartsWith("SD_", StringComparison.Ordinal))
        {
            return itemType == ItemType.SpecialDish;
        }

        //현재 프로젝트에서 정의하지 않은 ID 접두사
        return false;
    }

    //*현재 인벤토리에서 전달받은 아이템 ID와 일치하는 슬롯을 찾는 내부 메서드*
    private InventorySlotData FindSlot(string itemId)
    {
        //현재 슬롯 목록을 처음부터 끝까지 확인
        for (int index = 0; index < slots.Count; index++)
        {
            //현재 순서에서 확인할 슬롯 가져오기
            InventorySlotData slot = slots[index];

            //현재 슬롯이 null이면 비교할 수 없으므로 다음 슬롯으로 넘어가기
            if (slot == null) continue;

            //같은 아이템 찾았다면 해당 슬롯 반환
            if (slot.IsSameItem(itemId)) return slot;
        }

        //모든 슬롯 확인했지만 아이템이 없으면 null 반환
        return null;
    }

    //*현재 슬롯 목록을 아이템 종류와 ItemId 숫자 기준으로 정렬하는 메서드*
    //
    //정렬 결과:
    //IG_01 → IG_02 → IG_03...
    //DS_01 → DS_02 → DS_03...
    //SD_01 → SD_02 → SD_03...
    private void SortSlotsByItemTypeAndId()
    {
        //List.Sort()에 슬롯 비교 메서드를 전달
        //
        //CompareSlotsByItemTypeAndId()가
        //두 슬롯 중 어느 슬롯이 앞에 와야 하는지 결정
        slots.Sort(CompareSlotsByItemTypeAndId);
    }

    //*두 인벤토리 슬롯의 정렬 순서를 비교*
    //
    //1차 정렬:
    //재료 → 일반 음식 → 특별한 음식 → 탄 음식 → 음식물 쓰레기
    //
    //2차 정렬:
    //같은 종류 안에서 ID 숫자 오름차순
    private int CompareSlotsByItemTypeAndId(InventorySlotData firstSlot, InventorySlotData secondSlot)
    {
        //두 변수가 완전히 같은 슬롯을 가리키고 있다면
        //순서를 변경할 필요가 없으므로 0 반환
        if (ReferenceEquals(firstSlot, secondSlot)) return 0;

        //첫 번째 슬롯만 null이면 뒤로 이동
        if (firstSlot == null) return 1;

        //두 번째 슬롯만 null이면 뒤로 이동
        if (secondSlot == null) return -1;
        
        //첫 번째 슬롯의 ItemType 정렬 우선순위를 가져옴
        int firstGroupOrder = GetInventoryGroupSortOrder(firstSlot);

        //두 번째 슬롯의 ItemType 정렬 우선순위를 가져옴
        int secondGroupOrder = GetInventoryGroupSortOrder(secondSlot);

        //재료, 일반 음식, 특별한 음식, 탄 음식, 음식물 쓰레기 순서로 비교
        int groupComparison = firstGroupOrder.CompareTo(secondGroupOrder);

        //서로 다른 그룹이라면 그룹 비교 결과를 바로 반환
        if (groupComparison != 0) return groupComparison;

        //같은 그룹이라면 ID 마지막 숫자를 가져옴
        int firstIdNumber = GetItemIdNumber(firstSlot.ItemId);

        int secondIdNumber = GetItemIdNumber(secondSlot.ItemId);

        //ID 숫자를 오름차순으로 비교
        int idNumberComparison = firstIdNumber.CompareTo(secondIdNumber);

        //숫자가 다르면 숫자 비교 결과 반환
        if (idNumberComparison != 0) return idNumberComparison;

        //그룹과 ID 숫자까지 같다면
        //전체 ID 문자열을 마지막으로 비교
        return string.Compare(
            firstSlot.ItemId,
            secondSlot.ItemId,
            StringComparison.Ordinal
            );
    }

    //*인벤토리에 표시할 아이템 그룹의 정렬 순위를 반환*
    //
    //0: 재료
    //1: 일반 음식
    //2: 특별한 음식
    //3: 탄 음식
    //4: 음식물 쓰레기
    private int GetInventoryGroupSortOrder(InventorySlotData slot)
    {
        //슬롯이 없으면 정상 아이템보다 뒤로 이동
        if (slot == null) return int.MaxValue;

        string itemId = slot.ItemId;

        //탄 음식은 특별한 음식 다음으로 배치
        if (!string.IsNullOrWhiteSpace(itemId) && itemId.StartsWith("BD_", StringComparison.Ordinal)) return 3;

        //음식물 쓰레기는 인벤토리의 가장 뒤에 배치
        if (!string.IsNullOrWhiteSpace(itemId) && itemId.StartsWith("TD_", StringComparison.Ordinal)) return 4;
        
        //탄 음식과 음식물 쓰레기가 아니라면
        //기존 ItemType으로 정렬 순서를 결정
        switch (slot.ItemType)
        {
            //재료
            case ItemType.Ingredient:
                return 0;

            //일반 음식
            case ItemType.Dish:
                return 1;

            //특별한 음식
            case ItemType.SpecialDish:
                return 2;

            //정의되지 않은 타입은 가장 뒤로 이동
            default:
                return int.MaxValue;
        }
    }

    //*ItemId에서 마지막 숫자 부분을 가져오는 메서드*
    //
    //예:
    //IG_01 → 1
    //DS_12 → 12
    private int GetItemIdNumber(string itemId)
    {
        //ItemId가 null, 빈 문자열 또는 공백이라면
        //정상적인 ID가 아니므로 가장 큰 값 반환
        //
        //int.MaxValue를 반환하면
        //잘못된 ID가 정상 ID보다 뒤로 정렬됨
        if (string.IsNullOrWhiteSpace(itemId)) return int.MaxValue;
        
        //ItemId에서 마지막 '_' 문자의 위치를 찾음
        //
        //예:
        //IG_01에서 '_'의 위치를 가져옴
        int separatorIndex = itemId.LastIndexOf('_');

        //ItemId에 '_'가 없거나
        //'_' 뒤에 숫자로 사용할 문자가 없다면
        //정상적인 ID 형식이 아님
        if (separatorIndex < 0 || separatorIndex >= itemId.Length - 1) return int.MaxValue;
        
        //'_' 다음 위치부터 문자열 끝까지 잘라냄
        //
        //예:
        //IG_01 → "01"
        //DS_12 → "12"
        string numberText = itemId.Substring(separatorIndex + 1);

        //잘라낸 문자열을 int로 변환 시도
        if (int.TryParse(numberText, out int itemIdNumber))
        {
            //숫자 변환에 성공했다면 해당 숫자 반환
            return itemIdNumber;
        }

        //숫자 변환에 실패했다면
        //잘못된 ID이므로 정상 아이템보다 뒤로 정렬
        return int.MaxValue;
    }

    //*인벤토리가 변경되었음을 외부 스크립트에 알리는 메서드*
    private void NotifyInventoryChanged(InventoryChange change)
    {
        //상세 변경 정보가 필요한 UI 등에 전달
        OnInventoryChangedDetailed?.Invoke(change);

        //OnInventoryChanged 이벤트를 구독한 대상이 있다면 해당 이벤트 실시
        //?.Invoke를 사용해 구독자가 없어도 오류가 발생하지 않음
        OnInventoryChanged?.Invoke();
    }

    //SAVE
    public void Save(SaveData data)
    {
        data.inventory.Clear();
        foreach (InventorySlotData slot in slots)
        {
            if (slot != null && !slot.IsEmpty)
            {
                data.inventory.Add(slot);
            }
        }
    }

    //LOAD
    public void Load(SaveData data)
    {
        //기존 인벤토리 데이터를 먼저 전부 비움
        slots.Clear();

        //저장 데이터와 인벤토리 목록이 있을 때만 복원
        if (data != null && data.inventory != null)
        {
            //저장된 모든 인벤토리 슬롯을 순서대로 복원
            foreach (InventorySlotData savedSlot in data.inventory)
            {
                //비어 있거나 잘못된 슬롯은 복원하지 않음
                if (savedSlot == null || savedSlot.IsEmpty) continue;

                //저장 데이터 참조를 그대로 쓰지 않고
                //새로운 슬롯 데이터로 복사해서 추가
                InventorySlotData slot = new InventorySlotData(
                    savedSlot.ItemId,
                    savedSlot.Amount,
                    savedSlot.ItemType
                    );

                slots.Add(slot);
            }
        }

        //복원된 데이터도 평소 인벤토리 정렬 순서로 정리
        SortSlotsByItemTypeAndId();

        //이번 변경은 특정 아이템 하나가 아니라
        //인벤토리 전체가 저장 데이터로 교체된 경우
        InventoryChange change = new InventoryChange(
            InventoryChangeType.Loaded,
            string.Empty,
            0,
            0
            );

        //기존 두 이벤트 모두에 알림을 보내는 공용 메서드 호출
        NotifyInventoryChanged(change);
    }
}
