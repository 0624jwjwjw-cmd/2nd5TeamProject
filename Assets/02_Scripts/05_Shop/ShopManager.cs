//**상점에서 실제 구매 처리를 담당하는 Manager**
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    //*재료 구매를 시도하는 메서드*
    //itemId : 구매하려는 재료의 고유 ID
    //amount : 구매하려는 수량
    //
    //구매 성공 → true
    //구매 실패 → false
    public bool TryBuyIngredient(string itemId, int amount)
    {
        //ID가 비어 있으면 어떤 아이템인지 알 수 없으므로 구매 실패
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning("[ShopManager] 구매할 아이템 ID가 비어 있습니다.");
            return false;
        }

        //0개 이하 구매는 정상적인 요청이 아니므로 구매 실패
        if (amount <= 0)
        {
            Debug.LogWarning($"[ShopManager] 구매 수량은 1 이상이어야 합니다. 입력값: {amount}");
            return false;
        }

        //게임 데이터 Repository가 존재하는지 확인
        if (GameDataRepository.Instance == null)
        {
            Debug.LogError("[ShopManager] GameDataRepository가 존재하지 않습니다.");
            return false;
        }

        //인벤토리 Manager가 존재하는지 확인
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("[ShopManager] InventoryManager가 존재하지 않습니다.");
            return false;
        }

        //재화 Manager가 존재하는지 확인
        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("[ShopManager] CurrencyManager가 존재하지 않습니다.");
            return false;
        }

        //*ItemId를 이용해서 실제 IngredientData를 검색*
        if (!GameDataRepository.Instance.TryGetIngredient(itemId, out IngredientData ingredientData))
        {
            Debug.LogWarning($"[ShopManager] 재료 데이터를 찾을 수 없습니다. ID: {itemId}");
            return false;
        }

        //CSV나 SO 데이터에 잘못된 음수 가격이 들어온 경우 방어
        if (ingredientData.Price < 0)
        {
            Debug.LogError(
                $"[ShopManager] 재료 가격이 잘못되었습니다. " +
                $"ID: {itemId}, 가격: {ingredientData.Price}"
                );
            return false;
        }

        //*단가 × 구매 수량으로 최종 구매 가격 계산*
        int totalPrice = ingredientData.Price * amount;

        //돈을 먼저 사용하기 전에
        //인벤토리에 해당 수량이 실제로 들어갈 수 있는지 확인
        if (!InventoryManager.Instance.CanAddItem(itemId, amount))
        {
            Debug.LogWarning(
                $"[ShopManager] 인벤토리에 더 이상 추가할 수 없습니다. " +
                $"ID: {itemId}, 수량: {amount}"
                );
            return false;
        }

        //*현재 보유 Gold가 총 구매 가격보다 적으면 구매 실패*
        if (CurrencyManager.Instance.Gold < totalPrice)
        {
            Debug.LogWarning(
                $"[ShopManager] Gold가 부족합니다. " +
                $"필요: {totalPrice}, 보유: {CurrencyManager.Instance.Gold}"
                );
            return false;
        }

        //*모든 검사가 끝났으므로 실제 Gold 차감 시도*
        bool spendSucceeded = CurrencyManager.Instance.SpendGold(totalPrice);

        //예상하지 못한 이유로 Gold 차감에 실패했다면 구매 중단
        if (!spendSucceeded)
        {
            Debug.LogWarning("[ShopManager] Gold 차감에 실패했습니다.");
            return false;
        }

        //*구매한 재료를 실제 인벤토리에 추가*
        bool addSucceeded = InventoryManager.Instance.AddItem(itemId, amount);

        //인벤토리 추가가 예상치 못하게 실패했다면
        //이미 사용한 Gold를 다시 돌려줘서 데이터 불일치를 방지
        if (!addSucceeded)
        {
            CurrencyManager.Instance.AddGold(totalPrice);
            Debug.LogError(
                "[ShopManager] 인벤토리 추가에 실패하여 Gold를 환불했습니다."
                );
            return false;
        }

        //Gold 차감 + Inventory 추가까지 모두 정상적으로 끝남
        Debug.Log(
            $"[ShopManager] 구매 성공 | " +
            $"{ingredientData.IngredientName} x{amount} | " +
            $"사용 Gold: {totalPrice}"
            );
        return true;
    }
}

//ItemId 검사 → IngredientData 찾기 → 인벤토리에 들어갈 수 있는지 검사 → 돈이 충분한지 검사 → Gold 차감 → Inventory.AddItem()
//※ 돈 먼저 차감 후 인벤토리 확인했을때 최대라 추가 실패해서 돈만 차감되는 경우 방지