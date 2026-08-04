//**인벤토리 매니저**
using System;
using System.Collections.Generic;   //List사용
using UnityEngine;

//하나의 게임 오브젝트에 특정 컴포넌트를 오직 하나만 추가할 수 있게 제한하는 어트리뷰트
[DisallowMultipleComponent]

public class InventoryManager : MonoBehaviour
{
    //현재 게임에서 사용 중인 InventoryManager를
    //다른 스크립트가 쉽게 접근 가능하도록 제공하는 싱글톤 프로퍼티
    public static InventoryManager Instance { get; private set; }

    [Header("인벤토리 설정")]
    [Min(1)][SerializeField] private int maxStackSize = 99; //슬롯당 1 ~ 99로 기본값 설정

    [Header("현재 인벤토리 데이터")]
    //InventorySlotData를 여러 개 보관하는 실제 인벤토리 목록
    [SerializeField] private List<InventorySlotData> slots = new List<InventorySlotData>();
    
    //아이템 처음 들어온 순서를 계산하기 위한 카운터
    //새로 들어올때마다 1씩 증가하고, InventorySlotData의 acquiredOrder에 전달
    [SerializeField] private int acquiredOrderCounter;

    //인벤토리 내용이 변경되었을 때 실행되는 이벤트
    //InventoryUIController가 이 이벤트를 구독하면 아이템 추가ㆍ제거 후 UI를 자동으로 갱신 가능
    public event Action OnInventoryChanged;

    //프로퍼티
    //IReadOnlyList로 제공하기 때문에
    public IReadOnlyList<InventorySlotData> Slots => slots; //slots.Add()나 slots.Remove()직접 호출 불가
    public int MaxStackSize => maxStackSize;                //최대 스택 수량 외부에 제공
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

    //[Unity Inspector에서 값이 변경될 때 호출되는 메서드]
    //게임을 실행하지 않은 상태에서도
    //잘못된 설정값이 들어가지 않도록 보정
    private void OnValidate()
    {
        //최대 스택 수량이 1보다 작아지지 않도록 제한
        maxStackSize = Mathf.Max(1, maxStackSize);
    }

    //[Inspector나 저장 데이터로 들어온 인벤토리 목록을
    //실제 게임에서 사용하기 전에 정리하는 메서드]
    private void PrepareInventoryData()
    {
        if (slots == null)
        {
            //slots 리스트가 null이면 새로운 빈 리스트 생성
            slots = new List<InventorySlotData>();
        }

        //리스트 안에 null 슬롯이나 빈 슬롯이 있다면 제거
        slots.RemoveAll(slot => slot == null || slot.IsEmpty);

        //현재 슬롯 획득 순서 확인해서 acquiredOrderCounter 값을 다시 계산
        RecalculateAcquiredOrderCounter();

        //게임 시작 시 슬롯 목록을 아이템 ID 기준으로 정렬
        SortSlotsByItemId();
    }

    //[현재 슬롯 중 가장 큰 획득 순서를 찾아 acquiredOrderCounter에 저장하는 메서드]
    //저장 데이터를 불러왔을 때 새로운 아이템의 획득 순서가 기존 아이템과 겹치지 않도록 사용
    private void RecalculateAcquiredOrderCounter()
    {
        //획득 순서 카운터를 0으로 초기화
        acquiredOrderCounter = 0;

        //현재 인벤토리 슬롯을 처음부터 끝까지 확인
        for (int index = 0; index < slots.Count; index++)
        {
            //현재 순서에서 확인할 슬롯을 가져옴
            InventorySlotData slot = slots[index];

            //현재 슬롯의 획득 순서가 acquiredOrderCounter 보다 크면
            if (slot.AcquiredOrder > acquiredOrderCounter)
            {
                acquiredOrderCounter = slot.AcquiredOrder; //acquiredOrderCounter를 해당 값으로 변경
            }
        }
    }

    //[아이템을 인벤토리에 추가하는 메서드]
    //상점에서 재료를 구매하거나, 요리 시스템에서 완성된 음식을 지급할 때 사용
    public bool AddItem(string itemId, int amount)
    {
        //전달받은 아이템 ID가 비어 있는지 검사
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning("[InventoryManager] 아이템 ID가 비어 있어 추가할 수 없습니다.");
            //추가에 실패했으므로 false반환
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

        //동일한 아이템 슬롯이 이미 존재하면
        if (existingSlot != null)
        {
            //해당 슬롯의 수량만 증가
            existingSlot.AddAmount(amount);
        }
        else
        {
            //새로운 종류의 아이템이면 획득 순서 카운터 1 증가
            acquiredOrderCounter++;

            //전달받은 아이템 정보로 새로운 슬롯 데이터를 생성
            InventorySlotData newSlot =
                new InventorySlotData(
                    itemId,
                    amount,
                    acquiredOrderCounter
                    );

            //새로 만든 슬롯을 전체 인벤토리 목록에 추가
            slots.Add(newSlot);
        }

        //아이템이 추가된 뒤 아이템 ID 기준으로 자동 정렬
        SortSlotsByItemId();

        //인벤토리 데이터가 변경되었다는 이벤트 발생
        NotifyInventoryChanged();

        //아이템 추가에 성공했으므로 true 반환
        return true;
    }

    //[해당 아이템을 지정한 수량만큼 추가할 수 있는지 검사하는 메서드]
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

    //[아이템을 지정한 수량만큼 제거하는 메서드]
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

        //아이템을 제거한 결과 슬롯이 빈 슬롯이 되었는지 검사
        if (targetSlot.IsEmpty)
        {
            //수량이 0이 된 슬롯을 인벤토리 목록에서 제거
            slots.Remove(targetSlot);
        }

        //인벤토리가 변경되었다는 이벤트 발생시키기
        NotifyInventoryChanged();

        //제거 성공했으므로 true 반환
        return true;
    }

    //[특정 아이템을 필요한 수량만큼 보유하고 있는지 검사하는 메서드]
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

    //[특정 아이템을 현재 몇 개 보유하고 있는지 반환하는 메서드]
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

    //[특정 아이템을 추가할 수 있는 남은 수량을 반환하는 메서드]
    public int GetRemainingStackSpace(string itemId)
    {
        //아이템 ID가 비어 있다면 공간을 계산할 수 없으므로 0 반환
        if (string.IsNullOrWhiteSpace(itemId)) return 0;

        //현재 인벤토리에서 같은 아이템 슬롯 찾기
        InventorySlotData existingSlot = FindSlot(itemId);

        //같은 아이템 슬롯이 없다면 새 슬롯 전체를 사용할 수 있음
        if (existingSlot == null) return maxStackSize;

        //최대 스택 수량에서 현재 보유 수량을 빼서 남은 공간 계산
        int remainingSpace = maxStackSize - existingSlot.Amount;

        //음수 방지용으로 0과 더 큰 값을 반환
        return Mathf.Max(0, remainingSpace);
    }

    //[인벤토리 슬롯을 아이템 ID 기준으로 정렬하는 공개 메서드]
    public void SortByItemId()
    {
        //실제 슬롯 정렬 실행
        SortSlotsByItemId();
        //정렬 결과 UI 반영하도록 인벤토리 변경 이벤트 발생
        NotifyInventoryChanged();
    }

    //[현재 인벤토리에 들어 있는 모든 아이템을 제거하는 메서드]
    //데이터 초기화 또는 디버그 사용
    public void ClearInventory()
    {
        //이미 인벤토리가 비어 있으면 메서드 종료
        if (slots.Count == 0) return;

        //모든 슬롯 데이터를 리스트에서 제거
        slots.Clear();

        //획득 순서 카운터도 처음 값인 0으로 초기화
        acquiredOrderCounter = 0;

        //인벤토리가 변경되었다는 이벤트 발생
        NotifyInventoryChanged();
    }

    //[현재 인벤토리에서 전달받은 아이템 ID와
    //일치하는 슬롯을 찾는 내부 메서드]
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

    //현재 슬롯 목록을 아이템 ID 기준으로 정렬하는 내부 메서드
    private void SortSlotsByItemId()
    {
        //List의 Sort 기능을 사용해 슬롯 순서 변경
        slots.Sort(CompareSlotsByItemId);
    }

    //[두 슬롯의 아이템 ID를 비교해서 어떤 슬롯이 앞에 와야 하는지 결정하는 메서드]
    private int CompareSlotsByItemId(InventorySlotData firstSlot, InventorySlotData secondSlot)
    {
        //두 슬롯이 모두 같은 객체라면 순서를 바꿀 필요가 없으므로 0 반환
        if (ReferenceEquals(firstSlot, secondSlot)) return 0;

        //첫 번째 슬롯만 null이라면 첫 번째 슬롯을 뒤쪽으로 보내기 위해 1 반환
        if (firstSlot == null) return 1;

        //두 번째 슬롯만 null이라면 두 번째 슬롯을 뒤쪽으로 보내기 위해 -1 반환
        if (secondSlot == null) return -1;

        //두 슬롯의 아이템 ID를 정확한 문자열 기준으로 비교
        return string.Compare(firstSlot.ItemId, secondSlot.ItemId, StringComparison.Ordinal);
    }

    //[인벤토리가 변경되었음을 외부 스크립트에 알리는 메서드]
    private void NotifyInventoryChanged()
    {
        //OnInventoryChanged 이벤트를 구독한 대상이 있다면 해당 이벤트 실시
        //?.Invoke를 사용해 구독자가 없어도 오류가 발생하지 않음
        OnInventoryChanged?.Invoke();
    }
}
