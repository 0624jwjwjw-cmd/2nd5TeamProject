//**Dish 음식 배치와 인벤토리 차감을 처리하는 컨트롤러**
using System; //StringComparison 사용
using UnityEngine;

public class FoodPlaceController : MonoBehaviour
{
    public bool TryPlaceFood(FoodPlace foodPlace, string itemId)
    {
        //전달받은 접시가 없다면 배치할 수 없음
        if (foodPlace == null) return false;

        //음식 ID가 비어 있다면 배치할 수 없음
        if (string.IsNullOrWhiteSpace(itemId)) return false;

        //이미 음식이 놓여 있는 접시라면 배치하지 않음
        if (foodPlace.IsFilled) return false;

        //라이브 방송 중에는 새로운 음식을 접시에 배치하지 않음
        if (LiveManager.Instance != null && LiveManager.Instance.IsLive) return false;

        //인벤토리 매니저가 없다면
        //음식 보유 여부와 수량을 처리할 수 없음
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("[FoodPlaceController] InventoryManager가 존재하지 않습니다.");
            return false;
        }

        //현재 인벤토리에 해당 음식이 1개 이상 있는지 확인
        bool hasFood = InventoryManager.Instance.HasItem(itemId, 1);

        //보유하지 않은 음식이라면 배치하지 않음
        if (!hasFood)
        {
            Debug.LogWarning($"[FoodPlaceController] 보유하지 않은 음식입니다. ID: {itemId}");

            return false;
        }

        //FoodPlace에 음식 ID와 Sprite 표시 요청
        bool placed = foodPlace.TryPlace(itemId);

        //Sprite 검색 실패 또는 다른 문제로 배치에 실패한 경우
        if (!placed) return false;

        //접시 배치에 성공했으므로 인벤토리에서 음식 1개 차감
        bool removed = InventoryManager.Instance.RemoveItem(itemId, 1);

        //예상하지 못한 이유로 인벤토리 차감에 실패한 경우
        if (!removed)
        {
            //접시에 표시한 음식도 다시 제거하여 상태를 되돌림
            foodPlace.RemoveFood();

            Debug.LogError(
                $"[FoodPlaceController] 인벤토리 차감에 실패하여 " +
                $"접시 배치를 취소했습니다. ID: {itemId}"
                );

            return false;
        }

        //음식 표시와 인벤토리 차감 모두 성공
        return true;
    }

    //*접시에 배치된 음식을 인벤토리로 반환*
    public bool TryReturnFood(FoodPlace foodPlace)
    {
        //전달받은 접시가 없다면 반환할 수 없음
        if (foodPlace == null) return false;
        
        //현재 접시가 비어 있다면 반환할 음식이 없음
        if (!foodPlace.IsFilled) return false;        

        //방송이 시작된 후에는 접시의 음식을 취소할 수 없음
        if (LiveManager.Instance != null && LiveManager.Instance.IsLive) return false;
        
        //인벤토리 매니저가 없다면 음식을 반환할 수 없음
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("[FoodPlaceController] InventoryManager가 존재하지 않습니다.");
            return false;
        }

        //접시를 비우기 전에 현재 음식 ID 저장
        string itemId = foodPlace.ItemId;

        //음식 ID를 기준으로 원래 ItemType 확인
        if (!TryGetItemType(itemId, out ItemType itemType))
        {
            Debug.LogWarning($"[FoodPlaceController] ItemType을 확인할 수 없습니다. ID: {itemId}");
            return false;
        }

        //인벤토리로 음식 1개 반환
        bool returned = InventoryManager.Instance.AddItem(itemId, 1, itemType);

        //최대 수량 등의 이유로 반환에 실패했다면
        //접시의 음식은 그대로 유지
        if (!returned)
        {
            Debug.LogWarning(
                $"[FoodPlaceController] 음식을 인벤토리로 반환하지 못했습니다. " +
                $"ID: {itemId}"
                );

            return false;
        }

        //인벤토리 반환에 성공한 뒤 접시 이미지와 ItemId 제거
        foodPlace.RemoveFood();

        return true;
    }

    //*ItemId 앞부분으로 ItemType 확인*
    private bool TryGetItemType(string itemId, out ItemType itemType)
    {
        //실패할 경우를 대비해 기본값 설정
        itemType = default;

        //비어 있는 ID는 확인할 수 없음
        if (string.IsNullOrWhiteSpace(itemId)) return false;

        //재료 ID
        if (itemId.StartsWith("IG_", StringComparison.Ordinal))
        {
            itemType = ItemType.Ingredient;
            return true;
        }

        //특별한 음식 ID
        if (itemId.StartsWith(
                "SD_",
                StringComparison.Ordinal))
        {
            itemType = ItemType.SpecialDish;
            return true;
        }

        //일반 음식, 탄 음식, 음식물 쓰레기는 모두 Dish 타입
        if (itemId.StartsWith("DS_", StringComparison.Ordinal) ||
            itemId.StartsWith("BD_", StringComparison.Ordinal) ||
            itemId.StartsWith("TD_", StringComparison.Ordinal))
        {
            itemType = ItemType.Dish;
            return true;
        }

        //프로젝트에서 사용하지 않는 ID 형식
        return false;
    }
}
