//**상점에서 실제 구매 처리를 담당하는 Manager**
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    //*재료 구매를 시도하는 메서드*
    //itemId : 구매하려는 재료의 고유 ID
    //amount : 구매하려는 수량
    //
    //구매 성공 → true
    //구매 실패 → false

    //*장바구니 전체 재료 구매*
    public bool TryBuyCart(IEnumerable<ShopCartItemData> cartItems)
    {
        //장바구니 데이터가 없으면 구매 불가
        if (cartItems == null) return false;

        //여러 번 검사/처리하기 위해 List로 복사
        List<ShopCartItemData> items = new List<ShopCartItemData>(cartItems);

        //장바구니가 비어있으면 구매하지 않음
        if (items.Count == 0) return false;

        //필요한 Manager 확인
        if (InventoryManager.Instance == null ||
            CurrencyManager.Instance == null ||
            GameDataRepository.Instance == null)
        {
            Debug.LogError("[ShopManager] 구매에 필요한 Manager가 없습니다.");
            return false;
        }

        long totalPrice = 0;

        //돈을 쓰기 전에 장바구니 전체를 먼저 검사
        foreach (ShopCartItemData cartItem in items)
        {
            if (cartItem == null ||
                cartItem.Data == null ||
                cartItem.Amount <= 0)
            {
                return false;
            }

            string itemId = cartItem.Data.ID;

            //실제 재료 데이터 확인
            if (!GameDataRepository.Instance.TryGetIngredient(
                itemId,
                out IngredientData ingredientData))
            {
                return false;
            }

            //가격 데이터 방어
            if (ingredientData.Price < 0)
            {
                return false;
            }

            //이 재료가 인벤토리에 전부 들어갈 수 있는지 검사
            if (!InventoryManager.Instance.CanAddItem(itemId, cartItem.Amount))
            {
                Debug.LogWarning($"[ShopManager] 인벤토리 공간 부족 | {itemId}");
                return false;
            }

            //전체 가격 누적
            totalPrice += (long)ingredientData.Price * cartItem.Amount;
        }

        //int 범위를 벗어나는 비정상 가격 방어
        if (totalPrice > int.MaxValue)
        {
            Debug.LogError("[ShopManager] 총 구매 가격이 너무 큽니다.");
            return false;
        }

        int finalPrice = (int)totalPrice;

        //Gold 부족
        if (CurrencyManager.Instance.Gold < finalPrice)
        {
            Debug.LogWarning(
                $"[ShopManager] Gold 부족 | " +
                $"필요: {finalPrice}, 보유: {CurrencyManager.Instance.Gold}"
                );
            return false;
        }

        //모든 검사가 끝난 뒤 Gold 한 번만 차감
        if (!CurrencyManager.Instance.SpendGold(finalPrice)) return false;

        //실제로 인벤토리에 추가 완료된 항목 기록
        List<ShopCartItemData> addedItems = new List<ShopCartItemData>();

        foreach (ShopCartItemData cartItem in items)
        {
            bool addSucceeded =
                InventoryManager.Instance.AddItem(
                    cartItem.Data.ID,
                    cartItem.Amount,
                    ItemType.Ingredient
                    );

            //예상하지 못한 추가 실패
            if (!addSucceeded)
            {
                //이미 추가된 재료 원상복구
                foreach (ShopCartItemData addedItem in addedItems)
                {
                    InventoryManager.Instance.RemoveItem(
                        addedItem.Data.ID,
                        addedItem.Amount
                        );
                }

                //사용한 Gold도 환불
                CurrencyManager.Instance.AddGold(finalPrice);

                Debug.LogError("[ShopManager] 구매 실패 → 인벤토리와 Gold를 복구했습니다.");
                return false;
            }

            addedItems.Add(cartItem);
        }

        Debug.Log($"[ShopManager] 장바구니 구매 성공 | " + $"총 가격: {finalPrice} G");
        return true;
    }
}

//ItemId 검사 → IngredientData 찾기 → 인벤토리에 들어갈 수 있는지 검사 → 돈이 충분한지 검사 → Gold 차감 → Inventory.AddItem()
//※ 돈 먼저 차감 후 인벤토리 확인했을때 최대라 추가 실패해서 돈만 차감되는 경우 방지