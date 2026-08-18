//**상점에서 재료 하나의 정보를 표시하고 구매 버튼 입력을 처리하는 UI**
using TMPro;            //TMP_Text 사용
using UnityEngine;      
using UnityEngine.UI;   //Image, Button 사용

public class ShopItemUI : MonoBehaviour
{
    //*UI*
    [SerializeField] private Image itemIconImage;       //현재 재료의 아이콘을 표시하는 Image
    [SerializeField] private TMP_Text itemNameText;     //현재 재료의 이름을 표시하는 TMP Text
    [SerializeField] private TMP_Text priceText;        //현재 재료의 가격을 표시하는 TMP Text
    [SerializeField] private Button addButton;          //현재 재료를 장바구니에 추가할 때 사용하는 버튼

    //*데이터*
    //현재 이 ShopItemUI가 담당하고 있는 재료 데이터
    //Inspector에서 직접 넣는 것이 아니라
    //나중에 ShopUIManager가 Initialize()를 통해 넣어줄 예정이므로 SerializeField 사용 X
    private IngredientData ingredientData;

    //*장바구니*
    //재료 클릭 시 실제 장바구니에 추가 요청을 전달할 Controller
    //ShopItemUI가 직접 Gold 차감이나 Inventory 추가를 하지 않음
    private ShopCartController shopCartController;

    private void Awake()
    {
        //장바구니 추가 버튼이 Inspector에서 연결되지 않은 경우
        if (addButton == null)
        {
            Debug.LogError("[ShopItemUI] AddButton이 연결되어 있지 않습니다.");
            return;
        }

        //버튼을 클릭했을 때
        //OnClickAddButton() 실행
        addButton.onClick.AddListener(OnClickAddButton);

        //아직 어떤 재료를 담당하는지 모르기 때문에
        //구매 버튼을 비활성화
        addButton.interactable = false;
    }

    //*해당 GameObject가 파괴될 때 실행*
    private void OnDestroy()
    {
        //Button이 존재하는 경우에만 실행
        if (addButton != null)
        {
            //Awake()에서 등록한 버튼 이벤트 제거
            //오브젝트가 사라졌는데 이벤트 연결이 남는 것을 방지
            addButton.onClick.RemoveListener(OnClickAddButton);
        }
    }

    //*초기화*
    //이 ShopItemUI가 어떤 재료를 표시할지 설정하는 메서드
    //data              : 표시할 IngredientData
    //cartController    : 클릭했을 때 재료를 추가할 장바구니
    public void Initialize(IngredientData data, ShopCartController cartController)
    {
        //전달받은 IngredientData가 없는 경우
        if (data == null)
        {
            Debug.LogError("[ShopItemUI] IngredientData가 null입니다.");
            return;
        }

        //전달받은 ShopCartController가 없는 경우
        if (cartController == null)
        {
            Debug.LogError("[ShopItemUI] ShopCartController가 null입니다.");
            return;
        }

        //현재 슬롯이 담당할 IngredientData 저장
        ingredientData = data;

        //재료 클릭 시 사용할 장바구니 저장
        shopCartController = cartController;

        //받은 IngredientData를 이용해서 화면의 아이콘 / 이름 / 가격을 갱신
        RefreshUI();

        //정상적으로 초기화되었기 때문에 구매 버튼 활성화
        if (addButton != null)
        {
            addButton.interactable = true;
        }
    }

    //*UI 갱신*
    //현재 IngredientData를 기준으로 UI를 갱신하는 메서드
    private void RefreshUI()
    {
        //표시할 정보가 없으면 종료
        if (ingredientData == null) return;

        //재료 이름 표시
        if (itemNameText != null)
        {
            itemNameText.text = ingredientData.IngredientName;
        }

        //재료 가격 표시
        if (priceText != null)
        {
            priceText.text = $"{ingredientData.Price} G";
        }

        //재료 아이콘 표시
        if (itemIconImage != null)
        {
            //ItemVisualRepository가 아직 생성되지 않은 경우
            if (ItemVisualRepository.Instance == null)
            {
                Debug.LogError("[ShopItemUI] ItemVisualRepository가 존재하지 않습니다.");

                itemIconImage.sprite = null;

                return;
            }

            //현재 재료의 ID로 아이콘 검색
            bool iconFound =
                ItemVisualRepository.Instance.TryGetIcon(
                    ingredientData.ID,
                    out Sprite icon
                    );

            //아이콘 검색 성공
            if (iconFound)
            {
                itemIconImage.sprite = icon;
            }
            //아이콘 검색 실패
            else
            {
                itemIconImage.sprite = null;

                Debug.LogWarning($"[ShopItemUI] 아이콘을 찾을 수 없습니다. " + $"ID: {ingredientData.ID}");
            }
        }
    }

    //*재료 추가 버튼*
    //사용자가 상점의 재료를 클릭했을 때 실행
    private void OnClickAddButton()
    {
        //어떤 재료인지 모르면 추가 불가능
        if (ingredientData == null)
        {
            Debug.LogWarning("[ShopItemUI] 추가할 재료 데이터가 없습니다.");
            return;
        }

        //장바구니가 연결되지 않은 경우
        if (shopCartController == null)
        {
            Debug.LogError("[ShopItemUI] ShopCartController가 연결되어 있지 않습니다.");
            return;
        }

        //현재 재료를 장바구니에 1개 추가
        bool addSucceeded = shopCartController.AddIngredient(ingredientData);

        //장바구니 추가 실패
        if (!addSucceeded)
        {
            Debug.LogWarning(
                $"[ShopItemUI] 장바구니 추가 실패 | " +
                $"{ingredientData.IngredientName}"
                );
            return;
        }

        //추가 성공
        Debug.Log(
            $"[ShopItemUI] 장바구니 추가 성공 | " +
            $"{ingredientData.IngredientName} | " +
            $"현재 수량: " +
            $"{shopCartController.GetQuantity(ingredientData.ID)}"
            );
    }
}
