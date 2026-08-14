//**InventorySlotUI를 생성하고 재사용하기 위한 오브젝트 풀**
//
//실제 Get / Release / 생성 / 보관 로직은
//공용 ComponentPool<T>가 담당
//
//InventorySlotPool은
//인벤토리에서 사용할 InventorySlotUI Prefab을
//공용 Pool과 연결하는 역할만 담당
using UnityEngine;

//같은 게임 오브젝트에 해당 스크립트가 여러개 붙는거 방지
[DisallowMultipleComponent]

public sealed class InventorySlotPool : MonoBehaviour
{
    [Header("Inventory Slot Pool")]
    //인벤토리에서 반복해서 사용할 Slot UI 원본 Prefab
    [SerializeField] private InventorySlotUI slotPrefab;

    //사용하지 않는 InventorySlotUI를 보관할 전용 부모
    //Hierarchy의 InventoryPanel/PoolRoot를 연결
    [SerializeField] private Transform poolRoot;

    //실제 Pooling 기능을 담당하는 공용 Generic Pool
    //
    //여기서는 T가 InventorySlotUI가 됨
    private ComponentPool<InventorySlotUI> pool;

    private void Awake()
    {
        //Inspector에 Slot Prefab이 연결되지 않았다면
        //Pool을 만들 수 없으므로 오류 처리
        if (slotPrefab == null)
        {
            Debug.LogError("[InventorySlotPool] Slot Prefab이 연결되지 않았습니다.");
            return;
        }

        //Inspector에 PoolRoot가 연결되지 않았다면
        //반환된 Slot을 보관할 위치가 없으므로 Pool을 생성하지 않음
        if (poolRoot == null)
        {
            Debug.LogError("[InventorySlotPool] Pool Root가 연결되지 않았습니다.");
            return;
        }

        //InventorySlotUI를 관리하는 공용 Pool 생성
        //
        //slotPrefab
        //→ 어떤 Prefab을 재사용할지
        //
        //poolRoot
        //→ 사용하지 않는 Slot을 어디에 보관할지
        pool = new ComponentPool<InventorySlotUI>(
            slotPrefab,
            poolRoot
        );
    }

    //*사용할 InventorySlotUI 가져오기*
    public InventorySlotUI GetSlot(Transform parent)
    {
        //공용 Pool이 생성되지 않았다면
        //Slot을 가져올 수 없으므로 실패 처리
        if (pool == null)
        {
            Debug.LogError("[InventorySlotPool] Pool이 초기화되지 않았습니다.");
            return null;
        }

        //실제 Get / 생성 / 재사용 처리는 공용 ComponentPool이 담당
        return pool.Get(parent);
    }

    //*사용이 끝난 InventorySlotUI를 Pool에 반환*
    public void ReleaseSlot(InventorySlotUI slot)
    {
        //반환할 Slot이 없다면
        //Pool에 넣을 대상도 없으므로 종료
        if (slot == null) return;

        //공용 Pool이 초기화되지 않았다면
        //반환 처리를 할 수 없으므로 오류 출력
        if (pool == null)
        {
            Debug.LogError("[InventorySlotPool] Pool이 초기화되지 않았습니다.");
            return;
        }

        //이전에 표시하던 아이템 정보와 선택 상태를 초기화
        //다음에 다른 아이템 Slot으로 재사용될 때 이전 정보가 남는 것을 방지
        slot.ClearSlot();

        //실제 비활성화 / 부모 이동 / Stack 보관은 공용 ComponentPool이 담당
        pool.Release(slot);
    }
}
