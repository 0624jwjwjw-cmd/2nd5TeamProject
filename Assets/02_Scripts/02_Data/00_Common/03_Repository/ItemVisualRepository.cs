//**아이템 ID를 이용해 Sprite와 Prefab을 빠르게 검색하는 Repository**
using System;                       //StringComparer 사용
using System.Collections.Generic;   //Dictionary<TKey, TValue> 사용
using UnityEngine;

//같은 게임 오브젝트에 해당 스크립트가 여러개 붙는거 방지
[DisallowMultipleComponent]

public sealed class ItemVisualRepository : MonoBehaviour, IItemVisualRepository
{
    //Singleton
    //다른 시스템에서 ItemVisualRepository에 접근 가능하도록 현재 인스턴스를 저장
    public static ItemVisualRepository Instance { get; private set; }

    //Inspector 연결
    //게임에서 사용할 재료 / 일반 요리 / 특별 요리 Prefab 목록을 여기서 가져옴
    [Header("아이템 Visual Catalog")]
    [SerializeField] private ItemVisualCatalog catalog;

    //런타임 검색 Dictionary
    //아이템 ID를 Key로 사용해서 해당 아이템의 Sprite와 Prefab 정보를 저장
    //예)
    //
    //"IG_01"
    //   ↓
    //ItemVisualInfo
    //├── Icon = 빵 Sprite
    //└── Prefab = IG_01_빵 Prefab
    //
    //StringComparer.Ordinal은
    //ID 문자열을 정확하게 비교하도록 함
    private readonly Dictionary<string, ItemVisualInfo> visualLookup = new Dictionary<string, ItemVisualInfo>(StringComparer.Ordinal);

    //초기화 상태
    //Repository 초기화가 끝났는지 외부에서 확인 가능
    public bool IsInitialized { get; private set; }

    //내부 Visual 데이터
    //하나의 아이템 ID에 필요한 Visual 정보를 묶어서 보관
    //Dictionary를
    //Dictionary<string, Sprite>
    //Dictionary<string, GameObject>
    //두 개 만드는 대신
    //
    //Dictionary<string, ItemVisualInfo>
    //하나만 사용하기 위한 구조
    private readonly struct ItemVisualInfo
    {
        public Sprite Icon { get; }                             //인벤토리 / 상점 / 도감 UI에 사용할 Sprite
        public GameObject Prefab { get; }                       //실제 월드에 생성할 때 사용할 Prefab
        public ItemVisualInfo(Sprite icon, GameObject prefab)   //Visual 정보 생성
        {
            Icon = icon;
            Prefab = prefab;
        }
    }

    private void Awake()
    {
        //이미 다른 ItemVisualRepository가 존재한다면
        //현재 중복 오브젝트 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        //현재 컴포넌트를 Singleton Instance로 등록
        Instance = this;

        //ItemVisualCatalog에 등록된 아이콘과 프리팹을
        //ID 검색용 Dictionary로 변환
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

    private void OnDestroy()
    {
        //파괴되는 객체가 현재 Singleton 객체일 때만 Instance를 초기화
        if (Instance == this) Instance = null;
    }

    //Awake에서 Singleton 등록 후 한 번 호출
    //ItemVisualCatalog를 검색용 Dictionary로 변환
    private void InitializeRepository()
    {
        //이미 초기화했으면 중복초기화 방지
        if (IsInitialized) return;

        //Catalog가 Inspector에서 연결되지 않았다면 Repository를 만들 수 없으므로 중단
        if (catalog == null)
        {
            Debug.LogError("[ItemVisualRepository] ItemVisualCatalog가 연결되지 않았습니다.");
            return;
        }

        //이전 검색 데이터 모두 제거
        //현재는 최초 초기화 한 번이지만 혹시 재초기화 구조가 추가되더라도 안전하도록 초기화
        visualLookup.Clear();

        RegisterIngredientPrefabs();                        //재료 Prefab 등록
        RegisterDishPrefabs(catalog.DishPrefabs);           //일반 요리 Prefab 등록
        RegisterDishPrefabs(catalog.SpecialDishPrefabs);    //특별 요리 Prefab 등록

        //모든 등록 과정이 끝났으므로 Repository 사용 가능 상태로 변경
        IsInitialized = true;

        Debug.Log($"[ItemVisualRepository] 초기화 완료 | 등록 Visual: {visualLookup.Count}");
    }

    //재료 Prefab 등록
    private void RegisterIngredientPrefabs()
    {
        //Catalog에 등록된 재료 Prefab을 처음부터 끝까지 확인
        for (int i = 0; i < catalog.IngredientPrefabs.Count; i++)
        {
            IngredientBase ingredientPrefab = catalog.IngredientPrefabs[i];

            //Prefab 참조가 비어있다면 건너뜀
            if (ingredientPrefab == null)
            {
                Debug.LogWarning($"[ItemVisualRepository] 재료 Prefab {i}번이 비어 있습니다.");
                continue;
            }

            //IngredientBase에 IngredientData가 연결되지 않았다면 ID를 알아낼 수 없으므로 등록 불가능
            if (ingredientPrefab.Data == null)
            {
                Debug.LogError(
                    $"[ItemVisualRepository] " + 
                    $"{ingredientPrefab.name}에 IngredientData가 연결되지 않았습니다."
                    );
                continue;
            }

            //중요
            //ingredientPrefab.ID가 아닌 ingredintPrefab.Data.ID를 사용
            //IngredientBase.ID는 Awake 이후 초기화되지만
            //지금 Catalog가 가지고 있는 것은 Prefab Asset이기 때문
            string itemId = ingredientPrefab.Data.ID;

            //Prefab Root에 붙어있는 SpriteRenderer를 가져옴
            //게임 플레이 도중 반복 실행하는 것이 아니라 Repository 초기화 시 한 번만 실행
            SpriteRenderer spriteRenderer = ingredientPrefab.GetComponent<SpriteRenderer>();

            //SpriteRenderer가 없다면 인벤토리 아이콘을 가져올 수 없으므로 경고
            if (spriteRenderer == null)
            {
                Debug.LogWarning(
                    $"[ItemVisualRepository] " + 
                    $"{ingredientPrefab.name}에 SpriteRenderer가 없습니다."
                    );
                continue;
            }

            //Dictionary 등록
            RegisterVisual(itemId, spriteRenderer.sprite, ingredientPrefab.gameObject);
        }
    }

    //요리 Prefab 등록
    //일반 요리와 특별 요리 모두 DishBase를 사용하므로 하나의 메서드로 처리
    private void RegisterDishPrefabs(IReadOnlyList<DishBase> dishPrefabs)
    {
        for (int i = 0; i < dishPrefabs.Count; i++)
        {
            DishBase dishPrefab = dishPrefabs[i];

            //Prefab이 비어있다면 건너뜀
            if (dishPrefab == null)
            {
                Debug.LogWarning($"[ItemVisualRepository] 음식 Prefab {i}번이 비어 있습니다.");
                continue;
            }

            //DishData가 연결되어 있어야 해당 음식의 ID를 알아낼 수 있음
            if (dishPrefab.Data == null)
            {
                Debug.LogError(
                    $"[ItemVisualRepository] " +
                    $"{dishPrefab.name}에 DishData가 연결되지 않았습니다."
                    );
                continue;
            }

            //Prefab Asset에서는 DishBase.ID가 아니라 원본 DishData.ID를 사용
            string itemId = dishPrefab.Data.ID;

            //음식 Prefab Root의 SpriteRenderer 검색
            SpriteRenderer spriteRenderer = dishPrefab.GetComponent<SpriteRenderer>();

            if (spriteRenderer == null)
            {
                Debug.LogWarning(
                    $"[ItemVisualRepository] " +
                    $"{dishPrefab.name}에 SpriteRenderer가 없습니다."
                    );
                continue;
            }

            //Dictionary 등록
            RegisterVisual(itemId, spriteRenderer.sprite, dishPrefab.gameObject);
        }
    }

    //Dictionary 등록
    private void RegisterVisual(string itemId, Sprite icon, GameObject prefab)
    {
        //ID가 비어있으면 Dictionary Key로 사용할 수 없으므로 방어
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogError("[ItemVisualRepository] ID가 비어 있는 Visual은 등록할 수 없습니다.");
            return;
        }

        //같은 ID가 이미 등록되어 있다면
        //어떤 Prefab을 사용해야 할지 애매해지므로 등록하지 않음
        if (visualLookup.ContainsKey(itemId))
        {
            Debug.LogError($"[ItemVisualRepository] 중복 Visual ID 발견: {itemId}");
            return;
        }

        //하나의 ID에 Sprite와 Prefab을 함께 묶어서 등록
        visualLookup.Add(itemId, new ItemVisualInfo(icon, prefab));
    }

    //아이콘 검색
    public bool TryGetIcon(string itemId, out Sprite icon)
    {
        //기본 반환값 null로 설정
        icon = null;

        //초기화가 끝나지 않았다면 검색하지 않음
        if (!IsInitialized) return false;

        //잘못된 ID 방어
        if (string.IsNullOrWhiteSpace(itemId)) return false;

        //Dictionary에서 ID 검색
        if (!visualLookup.TryGetValue(itemId, out ItemVisualInfo visualInfo)) return false;

        //검색된 Visual 정보에서 Sprite 반환
        icon = visualInfo.Icon;

        return icon != null;
    }

    //프리팹 검색
    public bool TryGetPrefab(string itemId, out GameObject prefab)
    {
        //기본 반환값 null로 설정
        prefab = null;

        //초기화 전 사용 방지
        if (!IsInitialized) return false;

        //잘못된 ID 방어
        if (string.IsNullOrWhiteSpace(itemId)) return false;

        //Dictionary 검색
        if (!visualLookup.TryGetValue(itemId, out ItemVisualInfo visualInfo)) return false;

        //해당 아이템의 Prefab 반환
        prefab = visualInfo.Prefab;

        return prefab != null;
    }
}
