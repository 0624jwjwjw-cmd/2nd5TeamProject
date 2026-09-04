//**상점에서 실제 구매 처리를 담당하는 Manager**
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
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
            if (!InventoryManager.Instance.CanAddItem(itemId, cartItem.Amount)) return false;
            
            //전체 가격 누적
            totalPrice += (long)ingredientData.Price * cartItem.Amount;
        }

        //int 범위를 벗어나는 비정상 가격 방어
        if (totalPrice > int.MaxValue) return false;
        
        int finalPrice = (int)totalPrice;

        //Gold 부족
        if (CurrencyManager.Instance.Gold < finalPrice) return false;
        
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
                    InventoryManager.Instance.RemoveItem(addedItem.Data.ID, addedItem.Amount);
                }

                //사용한 Gold도 환불
                CurrencyManager.Instance.AddGold(finalPrice);

                return false;
            }
            addedItems.Add(cartItem);
        }
        return true;
    }
}