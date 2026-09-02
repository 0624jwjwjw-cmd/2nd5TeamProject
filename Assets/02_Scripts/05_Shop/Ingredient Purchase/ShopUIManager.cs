//**상점의 재료 목록을 생성하고
//ShopItem 클릭을 장바구니 시스템과 연결하는 UI 관리자**
using System.Collections;   //IEnumerator 초기화 코루틴 사용
using UnityEngine;

//같은 GameObject에 ShopUIManager가 여러 개 붙는 것을 방지
[DisallowMultipleComponent]

public class ShopUIManager : MonoBehaviour
{
    [Header("Shop UI")]
    //생성된 ShopItem들이 들어갈 부모 Transform
    [SerializeField] private Transform content;

    //재료 하나를 화면에 표시할 ShopItem Prefab
    [SerializeField] private ShopItemUI shopItemPrefab;

    [Header("Shop Data")]
    //상점에 표시할 전체 IngredientData 목록을 가지고 있는 데이터 카탈로그
    //
    //GameDataCatalog 안에
    //IngredientData[] ingredients가 있으므로
    //이 데이터를 기준으로 ShopItem을 생성
    [SerializeField] private GameDataCatalog gameDataCatalog;

    [Header("Shop Cart")]
    //장바구니 데이터와 수량을 관리하는 Controller
    //ShopItem을 클릭했을 때
    //ShopCartController.AddIngredient()를 호출하기 위해 연결
    [SerializeField] private ShopCartController shopCartController;

    //Runtime System 참조
    //아이템 ID를 이용해서
    //재료의 Sprite를 검색하는 Repository
    //
    //IngredientData에는 Sprite가 없기 때문에
    //아이콘은 ItemVisualRepository에서 가져옴
    private ItemVisualRepository itemVisualRepository;

    //ShopUIManager 초기화가 완료되었는지 기록
    private bool isInitialized;

    //ItemVisualRepository가 준비될 때까지 기다리는 코루틴
    private Coroutine initializeCoroutine;

    //*이 GameObject가 활성화될 때 Unity가 호출*
    private void OnEnable()
    {
        //이미 초기화가 끝났다면
        //ShopItem을 다시 생성하지 않음
        if (isInitialized) return;

        //Repository를 기다리는 코루틴이 실행 중이 아닐 때만 시작
        //OnEnable이 여러 번 호출되어도 코루틴이 중복 실행되지 않도록 방지
        if (initializeCoroutine == null)
        {
            initializeCoroutine = StartCoroutine(InitializeWhenReady());
        }
    }

    //*상점 UI 오브젝트가 비활성화될 때 호출*
    private void OnDisable()
    {
        //Repository를 기다리는 도중 상점 UI가 닫혔다면
        //실행 중인 초기화 코루틴 중단
        if (initializeCoroutine != null)
        {
            StopCoroutine(initializeCoroutine);
            initializeCoroutine = null;
        }
    }

    //*ItemVisualRepository가 준비될 때까지 기다리는 코루틴*
    private IEnumerator InitializeWhenReady()
    {
        //ItemVisualRepository가 생성되고
        //내부 Visual Dictionary 초기화까지 끝날 때까지 대기
        while (!IsRequiredSystemReady())
        {
            //다음 프레임까지 대기한 후 다시 확인
            yield return null;
        }

        //Repository 준비가 끝났으므로
        //현재 코루틴을 실행 중인 상태가 아닌 것으로 변경
        initializeCoroutine = null;

        //실제 상점 UI 초기화
        TryInitialize();
    }

    //*ShopUIManager가 사용하는 Repository 준비 상태 확인*
    private bool IsRequiredSystemReady()
    {
        //현재 게임에 존재하는 ItemVisualRepository 가져오기
        itemVisualRepository = ItemVisualRepository.Instance;

        //Repository가 존재하고
        //내부 Visual Dictionary 초기화까지 끝났을 때만 true
        return
            itemVisualRepository != null &&
            itemVisualRepository.IsInitialized;
    }

    //*초기화*
    private void TryInitialize()
    {
        //이미 초기화가 끝난 경우 중복 실행 방지
        if (isInitialized) return;

        //ItemVisualRepository가 준비되었는지 최종 확인
        if (!IsRequiredSystemReady()) return;

        //Inspector 연결 검사
        if (content == null)
        {
            Debug.LogError("[ShopUIManager] Content가 연결되지 않았습니다.");
            return;
        }

        if (shopItemPrefab == null)
        {
            Debug.LogError("[ShopUIManager] ShopItem Prefab이 연결되지 않았습니다.");
            return;
        }

        if (gameDataCatalog == null)
        {
            Debug.LogError("[ShopUIManager] GameDataCatalog가 연결되지 않았습니다.");
            return;
        }

        if (shopCartController == null)
        {
            Debug.LogError("[ShopUIManager] ShopCartController가 연결되지 않았습니다.");
            return;
        }

        //모든 준비가 끝났으므로
        //상점의 전체 재료 ShopItem 생성
        BuildShopItems();

        //초기화 완료 기록
        //이후 OnEnable이나 Start가 다시 실행돼도
        //ShopItem이 중복 생성되지 않음
        isInitialized = true;

        //정상적으로 몇 개의 재료 데이터를 기준으로
        //상점 UI를 생성했는지 Console에서 확인
        Debug.Log(
            $"[ShopUIManager] 초기화 완료 | " +
            $"재료 데이터: {gameDataCatalog.Ingredients.Count}개"
            );
    }

    //*ShopItem 생성*
    private void BuildShopItems()
    {
        //GameDataCatalog에 등록되어 있는
        //모든 IngredientData를 순서대로 확인
        for (int i = 0; i < gameDataCatalog.Ingredients.Count; i++)
        {
            //현재 순서의 IngredientData 가져오기
            IngredientData ingredientData = gameDataCatalog.Ingredients[i];

            //배열 안에 null 데이터가 있다면
            //해당 데이터는 ShopItem으로 만들 수 없으므로 건너뜀
            if (ingredientData == null)
            {
                Debug.LogWarning(
                    $"[ShopUIManager] " +
                    $"Ingredient {i}번 데이터가 비어 있습니다."
                    );
                continue;
            }

            //현재 IngredientData의 ID를 사용해서
            //ItemVisualRepository에서 해당 재료 아이콘 검색
            bool iconFound = itemVisualRepository.TryGetIcon(ingredientData.ID, out Sprite icon);

            //아이콘을 찾지 못했더라도
            //나머지 이름과 가격은 표시할 수 있으므로
            //ShopItem 생성 자체는 계속 진행
            if (!iconFound)
            {
                Debug.LogWarning(
                    $"[ShopUIManager] " +
                    $"아이콘을 찾지 못했습니다. ID: {ingredientData.ID}"
                    );
            }

            //ShopItem Prefab을 복제해서
            //Content의 자식으로 생성
            ShopItemUI shopItem = Instantiate(shopItemPrefab, content);

            //생성된 ShopItem에 필요한 정보 전달
            //
            //ingredientData
            //→ 이름, 가격, ID 등 실제 재료 데이터
            //
            //icon
            //→ ItemVisualRepository에서 찾은 Sprite
            //
            //HandleItemClicked
            //→ ShopItem을 눌렀을 때 호출할 메서드
            shopItem.Initialize(
                ingredientData,
                icon,
                HandleItemClicked
                );
        }
    }

    //*ShopItem 클릭 처리*
    private void HandleItemClicked(IngredientData ingredientData)
    {
        //잘못된 데이터가 전달되었다면
        //장바구니에 추가하지 않음
        if (ingredientData == null) return;

        //ShopCartController에게
        //현재 클릭한 재료를 장바구니에 추가하도록 요청
        //
        //같은 재료가 이미 들어 있다면
        //ShopCartController 내부에서 수량 +1 처리
        shopCartController.AddIngredient(ingredientData);
    }
}
