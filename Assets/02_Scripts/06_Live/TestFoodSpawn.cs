//**스튜디오 음식 인벤토리 테스트를 위해 일반 음식을 추가하는 스크립트**
using UnityEngine;

public class TestFoodSpawn : MonoBehaviour
{
    //테스트할 음식 한 종류당 추가 수량
    [Min(1)][SerializeField] private int amount = 1;

    //모든 오브젝트의 Awake가 끝난 후 한 번 실행
    private void Start()
    {
        //현재 씬에 InventoryManager가 없는 경우 오류 방지
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("[TestFoodSpawn] InventoryManager가 존재하지 않습니다.");
            return;
        }

        //DS_01부터 DS_20까지 일반 음식을 순서대로 추가
        for (int i = 1; i <= 20; i++)
        {
            //1 → DS_01, 2 → DS_02 형식으로 음식 ID 생성
            string itemId = $"DS_{i:00}";

            //DS_로 시작하는 ID는 일반 음식이므로 ItemType.Dish 사용
            InventoryManager.Instance.AddItem(itemId, amount, ItemType.Dish);
        }

        Debug.Log($"[TestFoodSpawn] 일반 음식 20종을 {amount}개씩 추가했습니다.");
    }
}