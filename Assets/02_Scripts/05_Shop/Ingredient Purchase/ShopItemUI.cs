//**상점에서 재료 하나의 정보를 표시하고 구매 버튼 입력을 처리하는 UI**
using TMPro;            //TMP_Text 사용
using UnityEngine;      
using UnityEngine.UI;   //Image, Button 사용

public class ShopItemUI : MonoBehaviour
{
    //*UI*
    [SerializeField] private Image itemIconImage;       //*현재 재료의 아이콘을 표시하는 Image*
    [SerializeField] private TMP_Text itemNameText;     //*현재 재료의 이름을 표시하는 TMP Text*
    [SerializeField] private TMP_Text priceText;        //*현재 재료의 가격을 표시하는 TMP Text*
    [SerializeField] private Button buyButton;          //*현재 재료를 구매할 때 사용하는 버튼*

    //*데이터*
    //*현재 이 ShopItemUI가 담당하고 있는 재료 데이터*
    //Inspector에서 직접 넣는 것이 아니라
    //나중에 ShopUIManager가 Initialize()를 통해 넣어줄 예정이므로 SerializeField 사용 X
    private IngredientData ingredientData;

    //*실제 구매 처리를 담당하는 ShopManager*
    //ShopItemUI가 직접
    //Gold를 차감하거나 Inventory를 수정하지 않고
    //ShopManager에게 구매 요청만 전달하기 위해 사용
    private ShopManager shopManager;

    private void Awake()
    {
        //구매 버튼이 Inspector에서 연결되지 않은 경우
        if (buyButton == null)
        {
            Debug.LogError("[ShopItemUI] BuyButton이 연결되어 있지 않습니다.");
            return;
        }

        //*구매 버튼을 클릭했을 때 OnClickBuyButton()이 실행되도록 이벤트 등록*
        buyButton.onClick.AddListener(OnClickBuyButton);

        //아직 IngredientData가 들어오지 않은 상태에서는
        //어떤 재료를 구매해야 하는지 알 수 없기 때문에
        //구매 버튼을 비활성화
        buyButton.interactable = false;
    }

    //*해당 GameObject가 파괴될 때 실행*
    private void OnDestroy()
    {
        //Button이 존재하는 경우에만 실행
        if (buyButton != null)
        {
            //*Awake()에서 등록한 버튼 이벤트 제거*
            //오브젝트가 사라졌는데 이벤트 연결이 남는 것을 방지
            buyButton.onClick.RemoveListener(OnClickBuyButton);
        }
    }

    //*초기화*
    //*이 ShopItemUI가 어떤 재료를 표시할지 설정하는 메서드*
    //data      : 이 슬롯에서 사용할 IngredientData
    //manager   : 구매 처리를 맡길 ShopManager
    //
    //나중에 ShopUIManager에서 호출할 예정
    public void Initialize(IngredientData data, ShopManager manager)
    {
        //전달받은 IngredientData가 없는 경우
        if (data == null)
        {
            Debug.LogError("[ShopItemUI] IngredientData가 null입니다.");
            return;
        }

        //전달받은 ShopManager가 없는 경우
        if (manager == null)
        {
            Debug.LogError("[ShopItemUI] ShopManager가 null입니다.");
            return;
        }

        //*전달받은 IngredientData 저장*
        //이제 이 ShopItemUI는 어떤 재료를 담당하는지 알고 있음
        ingredientData = data;

        //*구매 요청을 전달할 ShopManager 저장*
        shopManager = manager;

        //*받은 IngredientData를 이용해서
        //화면의 아이콘 / 이름 / 가격을 갱신*
        RefreshUI();

        //*정상적으로 초기화되었기 때문에 구매 버튼 활성화*
        if (buyButton != null)
        {
            buyButton.interactable = true;
        }
    }

    //*UI 갱신*
    //*현재 IngredientData를 기준으로 UI를 갱신하는 메서드*
    private void RefreshUI()
    {
        //IngredientData가 없으면 표시할 정보도 없으므로 종료
        if (ingredientData == null) return;

        //*재료 이름 표시*
        if (itemNameText != null)
        {
            itemNameText.text = ingredientData.IngredientName;
        }

        //*재료 가격 표시*
        if (priceText != null)
        {
            priceText.text = $"{ingredientData.Price} G";
        }

        //*재료 아이콘 표시*
        if (itemIconImage != null)
        {
            //ItemVisualRepository가 아직 생성되지 않은 경우
            if (ItemVisualRepository.Instance == null)
            {
                Debug.LogError("[ShopItemUI] ItemVisualRepository가 존재하지 않습니다.");
                return;
            }

            //현재 재료의 ID를 이용해
            //ItemVisualRepository에서 해당 재료의 Sprite를 검색
            if (ItemVisualRepository.Instance.TryGetIcon(
                ingredientData.ID,
                out Sprite icon))
            {
                //아이콘 검색에 성공했다면
                //현재 상점 슬롯의 Image에 Sprite 적용
                itemIconImage.sprite = icon;
            }
            else
            {
                //해당 ID와 연결된 아이콘을 찾지 못한 경우
                Debug.LogWarning(
                    $"[ShopItemUI] 아이콘을 찾을 수 없습니다. " +
                    $"ItemId: {ingredientData.ID}"
                    );

                //잘못된 이전 이미지가 남아있지 않도록 비움
                itemIconImage.sprite = null;
            }
        }
    }

    //*구매 버튼*
    //*사용자가 구매 버튼을 클릭했을 때 실행*
    private void OnClickBuyButton()
    {
        //현재 이 UI가 어떤 재료를 담당하는지 모르는 경우
        if (ingredientData == null)
        {
            Debug.LogWarning("[ShopItemUI] 구매할 재료 데이터가 없습니다.");
            return;
        }

        //구매 요청을 전달할 ShopManager가 없는 경우
        if (shopManager == null)
        {
            Debug.LogError("[ShopItemUI] ShopManager가 연결되어 있지 않습니다.");
            return;
        }

        //*ShopManager에게 실제 구매 요청*
        bool buySucceeded = shopManager.TryBuyIngredient(ingredientData.ID, 1);

        //구매에 실패한 경우
        if (!buySucceeded)
        {
            Debug.LogWarning(
                $"[ShopItemUI] 구매 실패 | " +
                $"{ingredientData.IngredientName}"
                );
            return;
        }

        //*여기까지 왔다면
        //ShopManager에서 Gold 차감과
        //Inventory 추가가 모두 성공한 상태*
        Debug.Log(
            $"[ShopItemUI] 구매 성공 | " +
            $"{ingredientData.IngredientName} x1"
            );
    }
}
