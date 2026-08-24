//**구매하기 버튼과 장바구니 전체 구매 처리를 연결하는 Controller**
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

        //구매 성공한 경우에만
        //장바구니 전체 초기화
        shopCartController.ClearCart();
    }

    //*현재 장바구니 상태에 따라 구매 버튼 활성화*
    private void UpdatePurchaseButtonState()
    {
        //구매 버튼이 연결되지 않았다면 처리하지 않음
        if (purchaseButton == null) return;

        //장바구니가 존재하고
        //재료가 하나 이상 들어 있을 때만 버튼 활성화
        purchaseButton.interactable = shopCartController != null && !shopCartController.IsEmpty;
    }
}