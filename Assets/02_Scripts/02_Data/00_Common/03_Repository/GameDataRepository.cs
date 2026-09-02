//**게임 실행 중 ID를 이용해 재료와 요리 데이터를 검색하는 Repository**
using System;                       //StringComparer 사용
using System.Collections.Generic;   //Dictionary와 HashSet 사용
using UnityEngine;

//같은 게임 오브젝트에 해당 스크립트가 여러개 붙는거 방지
[DisallowMultipleComponent]

public sealed class GameDataRepository : MonoBehaviour
{
    //게임 전체에 하나만 존재해야 하므로 싱글톤 형태로 관리
    public static GameDataRepository Instance { get; private set; }

    [Header("게임 데이터 목록")]

    //CSV 임포터가 생성한 ScriptableObject들을 등록해둔
    //GameDataCatalog 에셋을 Inspector에서 연결
    [SerializeField] private GameDataCatalog catalog;

    //재료 ID와 IngredientData를 연결하는 검색용 Dictionary
    public readonly Dictionary<string, IngredientData>
        ingredientLookup = new Dictionary<string, IngredientData>(StringComparer.Ordinal);

    //일반 요리 ID와 DishData를 연결하는 검색용 Dictionary
    public readonly Dictionary<string, DishData>
        dishLookup = new Dictionary<string, DishData>(StringComparer.Ordinal);

    //특별 요리 ID와 DishData를 연결하는 검색용 Dictionary
    public readonly Dictionary<string, DishData>
        specialDishLookup = new Dictionary<string, DishData>(StringComparer.Ordinal);

    //재료, 일반 요리, 특별 요리 전체에서 동일한 ID가 중복 등록되는 것을 검사하기 위한 HashSet
    private readonly HashSet<string>
        registeredItemIds = new HashSet<string>(StringComparer.Ordinal);

    //Repository가 초기화되었는지 기록
    public bool IsInitialized { get; private set; }

    //프로퍼티
    public int IngredientCount => ingredientLookup.Count;   //재료 개수
    public int DishCount => dishLookup.Count;               //일반 요리 개수
    public int SpecialDishCount => specialDishLookup.Count; //특별 요리 개수

    //요리 Dictionary에 등록된 값들만 외부에 읽기 전용으로 제공
    //public IEnumerable<DishData> Dishes => dishLookup.Values;


    private void Awake()
    {
        //이미 다른 GameDataRepository가 존재한다면
        //현재 중복 오브젝트 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        //현재 컴포넌트를 Singleton Instance로 등록
        Instance = this;

        //GameDataCatalog의 데이터를
        //검색용 Dictionary로 변환
        InitializeRepository();

        //Catalog 누락 등의 이유로 초기화에 실패했다면
        //잘못된 Repository를 Singleton으로 유지하지 않음
        if (!IsInitialized)
        {
            Instance = null;
            return;
        }

        //정상적으로 초기화된 Repository만
        //씬이 변경되어도 유지
        DontDestroyOnLoad(gameObject);
    }

    //Awake에서 Singleton 등록 후 한 번 호출
    //GameDataCatalog를 검색용 Dictionary로 변환
    private void InitializeRepository()
    {
        if (IsInitialized) return;  //초기화가 끝났다면 중복 작업 X
        //Catalog가 Inspector에 연결되지 않았다면 종료
        if (catalog == null)
        {
            Debug.LogError("[GameDataRepository] " + "GameDataCatalog가 연결되지 않았습니다.");
            return;
        }

        //이전 데이터가 남아 있을 수 있으니
        //모든 검색 Dictionary 먼저 비우기
        ingredientLookup.Clear();
        dishLookup.Clear();
        specialDishLookup.Clear();
        registeredItemIds.Clear();

        //Catalog의 재료 목록을 Dictionary에 등록
        RegisterIngredients();

        //Catalog의 일반 요리 목록을 Dictionary에 등록
        RegisterDishes();

        //Catalog의 특별 요리 목록을 Dictionary에 등록
        RegisterSpecialDishes();

        //모든 데이터 등록이 끝났음을 기록
        IsInitialized = true;

        //초기화 결과를 확인할 수 있도록 출력
        Debug.Log(
            $"[GameDataRepository] 초기화 완료 | " +
            $"재료: {IngredientCount}, " +
            $"일반 요리: {DishCount}, " +
            $"특별 요리: {SpecialDishCount}"
            );
    }

    //[Catalog에 등록된 재료 데이터를 Dictionary로 변환]
    private void RegisterIngredients()
    {
        //Catalog의 모든 재료 데이터를 순서대로 확인
        for (int i = 0; i < catalog.Ingredients.Count; i++)
        {
            //현재 순서의 재료 데이터를 가져옴
            IngredientData data = catalog.Ingredients[i];

            //비어 있는 Element라면 등록하지 않고 경고 출력
            if (data == null)
            {
                Debug.LogWarning($"[GameDataRepository]" + $"Ingredients의 Element {i}가 비어 있습니다.");
                continue;
            }

            //재료의 고유 ID를 가져옴
            string itemId = data.ID;

            //ID가 유효하고 전체 데이터에서 중복되지 않았는지 확인
            if (!TryReserveItemId(itemId, "재료")) continue;

            //재료 Dictionary에 등록
            ingredientLookup.Add(itemId, data);
        }
    }

    //[Catalog에 등록된 일반 요리 데이터를 Dictionary로 변환]
    private void RegisterDishes()
    {
        //Catalog의 모든 일반 요리 데이터를 순서대로 확인
        for (int i = 0; i < catalog.Dishes.Count; i++)
        {
            //현재 순서의 일반 요리 데이터를 가져옴
            DishData data = catalog.Dishes[i];

            //비어 있는 Element라면 등록하지 않고 경고 출력
            if (data == null)
            {
                Debug.LogWarning($"[GameDataRepository]" + $"Dishes의 Element {i}가 비어 있습니다.");
                continue;
            }

            //일반 요리의 고유 ID를 가져옴
            string itemId = data.ID;

            //ID가 유효하고 전체 데이터에서 중복되지 않았는지 확인
            if (!TryReserveItemId(itemId, "일반 요리")) continue;

            //일반 요리 Dictionary에 등록
            dishLookup.Add(itemId, data);
        }
    }

    //[Catalog에 등록된 특별 요리 데이터를 Dictionary로 변환]
    private void RegisterSpecialDishes()
    {
        //Catalog의 모든 특별 요리 데이터를 순서대로 확인
        for (int i = 0; i < catalog.SpecialDishes.Count; i++)
        {
            //현재 순서의 특별 요리 데이터를 가져옴
            DishData data = catalog.SpecialDishes[i];

            //비어 있는 Element라면 등록하지 않고 경고 출력
            if (data == null)
            {
                Debug.LogWarning($"[GameDataRepository]" + $"Special Dishes의 Element {i}가 비어 있습니다.");
                continue;
            }

            //특별 요리의 고유 ID를 가져옴
            string itemId = data.ID;

            //ID가 유효하고 전체 데이터에서 중복되지 않았는지 확인
            if (!TryReserveItemId(itemId, "특별 요리")) continue;

            //특별 요리 Dictionary에 등록
            specialDishLookup.Add(itemId, data);
        }
    }

    //ID가 비어 있거나 중복 되었는지 검사
    private bool TryReserveItemId(string itemId, string dataTypeName)
    {
        //ID가 null이거나 공백이라면 등록할 수 없음
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogError($"[GameDataRepository] " + $"{dataTypeName} 데이터에 ID가 없습니다.");
            return false;
        }

        //HashSet.Add는 새로운 값이면 true, 이미 같은 값이면 false 반환
        if (!registeredItemIds.Add(itemId))
        {
            Debug.LogError($"[GameDataRepository] " + $"중복 ID가 발견되었습니다: {itemId}");
            return false;
        }

        return true; //정상적인 ID면 등록 허용
    }

    //ID로 재료 데이터 검색
    public bool TryGetIngredient(string itemId, out IngredientData ingredientData)
    {
        //검색 실패 시 기본값을 null로 설정
        ingredientData = null;

        //초기화되지 않았거나 ID가 비어 있다면 검색하지 않음
        if (!IsInitialized || string.IsNullOrWhiteSpace(itemId)) return false;

        //Dictionary에서 ID 한 번에 검색
        return ingredientLookup.TryGetValue(itemId, out ingredientData);
    }

    //ID로 일반 요리 데이터를 검색
    public bool TryGetDish(string itemId, out DishData dishData)
    {
        //검색 실패 시 기본값을 null로 설정
        dishData = null;

        //초기화되지 않았거나 ID가 비어 있다면 검색하지 않음
        if (!IsInitialized || string.IsNullOrWhiteSpace(itemId)) return false;

        //Dictionary에서 ID 한 번에 검색
        return dishLookup.TryGetValue(itemId, out dishData);
    }

    //ID로 특별 요리 데이터를 검색
    public bool TryGetSpecialDish(string itemId, out DishData specialDishData)
    {
        //검색 실패 시 기본값을 null로 설정
        specialDishData = null;

        //Repository가 초기화되지 않았거나 ID가 비어 있다면 검색하지 않음
        if (!IsInitialized || string.IsNullOrWhiteSpace(itemId)) return false;

        //Dictionary에서 ID 한 번에 검색
        return specialDishLookup.TryGetValue(itemId, out specialDishData);
    }

    //Repository 오브젝트가 파괴될 때 Unity가 호출
    private void OnDestroy()
    {
        //현재 파괴되는 오브젝트가 싱글톤 인스턴스인 경우에만 정적 참조를 초기화
        if (Instance == this) Instance = null;
    }
}
