//**구매하기 버튼과 장바구니 전체 구매 처리를 연결하는 Controller**
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//같은 GameObject에 중복으로 붙는 것을 방지
[DisallowMultipleComponent]

public sealed class ShopPurchaseController : MonoBehaviour
{
    [Header("Shop")]
    //현재 장바구니의 재료와 수량을 관리하는 Controller
    [SerializeField] private ShopCartController shopCartController;

    //Gold 차감 + Inventory 추가를 실제로 처리하는 Manager
    [SerializeField] private ShopManager shopManager;

    [Header("UI")]
    [SerializeField] private Button purchaseButton;     //재료 구매를 실행하는 버튼

    //구매가 불가능한 이유를 표시할 Text
    [SerializeField] private TMP_Text purchaseMessageText;


    //*UI가 활성화될 때*
    private void OnEnable()
    {
        //구매 버튼이 연결되어 있다면
        if (purchaseButton != null)
        {
            //중복 등록 방지를 위해 기존 이벤트 제거
            purchaseButton.onClick.RemoveListener(HandlePurchaseClicked);

            //구매 버튼 클릭 이벤트 등록
            purchaseButton.onClick.AddListener(HandlePurchaseClicked);
        }

        //장바구니가 연결되어 있다면
        if (shopCartController != null)
        {
            //중복 이벤트 방지
            shopCartController.OnCartChanged -= UpdatePurchaseButtonState;

            //장바구니가 변경될 때마다
            //구매 버튼 활성화 상태도 갱신
            shopCartController.OnCartChanged += UpdatePurchaseButtonState;
        }

        //Gold 변경 이벤트
        if (CurrencyManager.Instance != null)
        {
            //중복 등록 방지
            CurrencyManager.Instance.OnRevenueChanged -= UpdatePurchaseButtonState;

            //Gold가 변경될 때마다
            //구매 버튼 활성화 여부 다시 검사
            CurrencyManager.Instance.OnRevenueChanged += UpdatePurchaseButtonState;
        }

        //처음 화면이 열렸을 때도 상태 갱신
        UpdatePurchaseButtonState();
    }

    //*UI가 비활성화될 때*
    private void OnDisable()
    {
        //버튼 이벤트 해제
        if (purchaseButton != null)
        {
            purchaseButton.onClick.RemoveListener(HandlePurchaseClicked);
        }

        //장바구니 이벤트 해제
        if (shopCartController != null)
        {
            shopCartController.OnCartChanged -= UpdatePurchaseButtonState;
        }

        //Gold 변경 이벤트 해제
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnRevenueChanged -= UpdatePurchaseButtonState;
        }
    }

    //*구매하기 버튼 클릭*
    private void HandlePurchaseClicked()
    {
        //필요한 시스템이 연결되지 않았다면 구매하지 않음
        if (shopCartController == null || shopManager == null) return;

        //장바구니가 비어 있다면 구매하지 않음
        if (shopCartController.IsEmpty) return;

        //장바구니 전체 구매 시도
        bool purchaseSucceeded = shopManager.TryBuyCart(shopCartController.CartItems);

        //구매 실패 시
        //장바구니는 그대로 유지
        if (!purchaseSucceeded) return;

        //실제 구매가 성공했을 때만 코인 효과음 재생
        SoundManager.Instance?.PlaySFX(SFXType.Coin);

        //구매 성공한 경우에만
        //장바구니 전체 초기화
        shopCartController.ClearCart();
    }

    //*현재 장바구니와 Gold를 기준으로 구매 버튼 활성화*
    private void UpdatePurchaseButtonState()
    {
        //구매 버튼이 연결되지 않았다면 처리하지 않음
        if (purchaseButton == null) return;

        //장바구니 Controller가 없다면
        //구매 버튼을 사용할 수 없음
        if (shopCartController == null)
        {
            purchaseButton.interactable = false;
            SetPurchaseMessage("장바구니 시스템을 찾을 수 없습니다.");
            return;
        }

        //CurrencyManager가 없다면
        //현재 Gold를 알 수 없으므로 구매 불가
        if (CurrencyManager.Instance == null)
        {
            purchaseButton.interactable = false;
            SetPurchaseMessage("재화 정보를 불러올 수 없습니다.");
            return;
        }

        //InventoryManager가 없다면
        //인벤토리에 들어갈 수 있는지 확인할 수 없으므로 구매 불가
        if (InventoryManager.Instance == null)
        {
            purchaseButton.interactable = false;
            SetPurchaseMessage("인벤토리 정보를 불러올 수 없습니다.");
            return;
        }

        //1. 장바구니가 비어있는지 검사
        if (shopCartController.IsEmpty)
        {
            purchaseButton.interactable = false;

            //장바구니가 비어있는 건 굳이
            //에러처럼 보여줄 필요 없으므로 문구 제거
            SetPurchaseMessage(string.Empty);

            return;
        }

        //2. Gold 부족 검사

        int totalPrice = shopCartController.GetTotalPrice();
        int currentGold = CurrencyManager.Instance.Gold;

        if (currentGold < totalPrice)
        {
            purchaseButton.interactable = false;

            int shortage = totalPrice - currentGold;

            SetPurchaseMessage($"{shortage}원 부족합니다.");

            return;
        }

        //3. 인벤토리 최대 보유량 검사
        if (!CanAddAllCartItems())
        {
            purchaseButton.interactable = false;

            SetPurchaseMessage("보유 한도를 초과했습니다.");

            return;
        }

        // 모든 조건 만족
        purchaseButton.interactable = true;
        SetPurchaseMessage(string.Empty);
    }

    //*현재 장바구니의 모든 재료가
    //인벤토리에 들어갈 수 있는지 확인*
    private bool CanAddAllCartItems()
    {
        //장바구니 Controller가 없으면 검사할 수 없음
        if (shopCartController == null) return false;
        
        //InventoryManager가 없으면
        //인벤토리 수량을 확인할 수 없음
        if (InventoryManager.Instance == null) return false;    

        //현재 장바구니에 들어있는
        //모든 재료를 하나씩 확인
        foreach (ShopCartItemData cartItem in shopCartController.CartItems)
        {
            //잘못된 장바구니 데이터 방어
            if (cartItem == null || cartItem.Data == null) return false;
            
            //현재 장바구니 재료의 ID
            string itemId = cartItem.Data.ID;

            //현재 장바구니에 담긴 구매 수량
            int amount = cartItem.Amount;

            //InventoryManager에게
            //"이 아이템을 이만큼 더 넣을 수 있어?"
            //라고 물어봄
            bool canAdd = InventoryManager.Instance.CanAddItem(itemId, amount);

            //하나라도 들어갈 수 없다면
            //전체 구매도 불가능
            if (!canAdd) return false;
        }

        //모든 재료가 정상적으로 들어갈 수 있음
        return true;
    }

    //*구매 안내 문구 변경*
    private void SetPurchaseMessage(string message)
    {
        //Text가 Inspector에 연결되지 않았다면
        //문구 표시만 생략
        if (purchaseMessageText == null) return;
        
        purchaseMessageText.text = message;
    }
}
