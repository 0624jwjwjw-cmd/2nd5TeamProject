//**ItemComponentPoolRegistry가 ID별 Prefab Pool을
//정상적으로 생성하고 재사용하는지 확인하기 위한 테스트 스크립트**
using UnityEngine;


//같은 GameObject에 테스트 스크립트가 중복으로 붙는 것을 방지
[DisallowMultipleComponent]

public sealed class ItemComponentPoolRegistryDebugTester : MonoBehaviour
{
    [Header("Pool Registry Test")]

    //Get()으로 가져온 재료 객체를
    //실제로 배치할 부모
    [SerializeField] private Transform spawnRoot;

    //Release()된 재료 객체들이
    //비활성화 상태로 대기할 부모
    [SerializeField] private Transform poolRoot;

    //재료 Prefab들을 ItemId별로 관리할 Registry
    private ItemComponentPoolRegistry<IngredientBase> ingredientRegistry;

    //현재 Get()으로 가져와 사용 중인 테스트 재료 객체
    //Release 테스트에서 같은 객체를 다시 Pool로 반환하기 위해 저장
    private IngredientBase currentIngredient;

    //현재 Get()으로 가져와 사용 중인 IG_04 테스트 재료 객체
    //IG_01과 서로 다른 ItemId Pool이 독립적으로 동작하는지 확인하기 위해 저장
    private IngredientBase currentIngredient04;
    private IngredientBase currentIngredient04Second;   //두 번째 테스트 재료 객체

    //*테스트용 Registry 초기화*
    private bool TryInitializeRegistry()
    {
        //이미 Registry가 만들어져 있다면
        //다시 생성하지 않고 그대로 사용
        if (ingredientRegistry != null) return true;

        //ItemId를 실제 Prefab으로 변환해 줄
        //Visual Repository 가져오기
        ItemVisualRepository visualRepository = ItemVisualRepository.Instance;

        //Repository가 없거나 아직 초기화되지 않았다면
        //Registry를 정상적으로 사용할 수 없음
        if (visualRepository == null ||
            !visualRepository.IsInitialized)
        {
            Debug.LogWarning("[PoolRegistryTest] ItemVisualRepository가 준비되지 않았습니다.");
            return false;
        }

        //Get()한 객체를 배치할 위치 검사
        if (spawnRoot == null)
        {
            Debug.LogWarning("[PoolRegistryTest] Spawn Root가 연결되지 않았습니다.");
            return false;
        }

        //Pool 대기 객체를 보관할 위치 검사
        if (poolRoot == null)
        {
            Debug.LogWarning("[PoolRegistryTest] Pool Root가 연결되지 않았습니다.");
            return false;
        }

        //IngredientBase 타입을 사용하는
        //ID 기반 Pool Registry 생성
        ingredientRegistry =
            new ItemComponentPoolRegistry<IngredientBase>(
                visualRepository,
                poolRoot
                );

        Debug.Log("[PoolRegistryTest] Ingredient Registry 초기화 완료");

        return true;
    }

    //*IG_01 재료 객체를 Pool에서 가져오는 테스트*
    [ContextMenu("IG_01 가져오기")]
    private void GetIngredient01()
    {
        //Registry가 아직 준비되지 않았다면
        //초기화를 시도하고, 실패하면 테스트 중단
        if (!TryInitializeRegistry()) return;

        //이미 Get()으로 가져와 사용 중인 객체가 있다면
        //참조를 덮어쓰지 않도록 새로운 Get 요청을 막음
        if (currentIngredient != null)
        {
            Debug.LogWarning("[PoolRegistryTest] 이미 사용 중인 IG_01 객체가 있습니다. 먼저 반환해주세요.");
            return;
        }

        //IG_01에 해당하는 IngredientBase 객체를
        //ID 기반 Registry를 통해 가져옴
        currentIngredient = ingredientRegistry.Get("IG_01", spawnRoot);

        //객체를 가져오지 못했다면
        //Prefab 등록이나 IngredientBase 컴포넌트 연결 문제일 수 있음
        if (currentIngredient == null)
        {
            Debug.LogWarning("[PoolRegistryTest] IG_01 객체를 가져오지 못했습니다.");
            return;
        }

        //정상적으로 가져왔는지 확인하기 위한 로그
        Debug.Log($"[PoolRegistryTest] IG_01 Get 성공: {currentIngredient.name}");
    }

    //*현재 사용 중인 IG_01 재료 객체를 Pool로 반환하는 테스트*
    [ContextMenu("IG_01 반환하기")]
    private void ReleaseIngredient01()
    {
        //Registry가 아직 준비되지 않았다면
        //반환 작업을 진행할 수 없으므로 종료
        if (!TryInitializeRegistry()) return;


        //현재 Get()으로 가져와 사용 중인 객체가 없다면
        //반환할 대상이 없는 상태
        if (currentIngredient == null)
        {
            Debug.LogWarning("[PoolRegistryTest] 반환할 IG_01 객체가 없습니다.");
            return;
        }

        //현재 객체를 IG_01 전용 Pool에 반환
        ingredientRegistry.Release("IG_01", currentIngredient);

        Debug.Log($"[PoolRegistryTest] IG_01 Release 성공: {currentIngredient.name}");

        //이미 Pool에 반환했으므로
        //테스트 스크립트에서는 더 이상 사용 중인 객체로 보관하지 않음
        currentIngredient = null;
    }

    //*IG_04 재료 객체를 Pool에서 가져오는 테스트*
    [ContextMenu("IG_04 가져오기")]
    private void GetIngredient04()
    {
        //Registry가 아직 준비되지 않았다면
        //초기화를 시도하고 실패하면 테스트 중단
        if (!TryInitializeRegistry()) return;

        //이미 IG_04 객체를 가져와 사용 중이라면
        //기존 참조가 덮어써지는 것을 방지
        if (currentIngredient04 != null)
        {
            Debug.LogWarning("[PoolRegistryTest] 이미 사용 중인 IG_04 객체가 있습니다. 먼저 반환해주세요.");
            return;
        }

        //같은 Ingredient Registry에 다른 ItemId인 IG_04를 요청
        //
        //IG_01 Pool과는 별개의
        //IG_04 전용 ComponentPool<IngredientBase>가 만들어짐
        currentIngredient04 = ingredientRegistry.Get("IG_04", spawnRoot);

        //Prefab이나 IngredientBase 컴포넌트를 찾지 못한 경우
        if (currentIngredient04 == null)
        {
            Debug.LogWarning("[PoolRegistryTest] IG_04 객체를 가져오지 못했습니다.");
            return;
        }

        //정상적으로 다른 ID의 객체를 가져왔는지 확인
        Debug.Log($"[PoolRegistryTest] IG_04 Get 성공: {currentIngredient04.name}");
    }

    //*같은 IG_04 객체를 동시에 2개 가져오는 테스트*
    [ContextMenu("IG_04 두 개 가져오기")]
    private void GetTwoIngredient04()
    {
        //Registry가 준비되지 않았다면 테스트 중단
        if (!TryInitializeRegistry()) return;

        //기존 테스트 객체가 남아 있다면
        //참조를 덮어쓰지 않도록 먼저 반환하도록 안내
        if (currentIngredient04 != null || currentIngredient04Second != null)
        {
            Debug.LogWarning("[PoolRegistryTest] 사용 중인 IG_04 객체가 있습니다. 먼저 반환해주세요.");
            return;
        }

        //같은 ItemId인 IG_04를 첫 번째로 요청
        currentIngredient04 = ingredientRegistry.Get("IG_04", spawnRoot);

        //같은 ItemId인 IG_04를 두 번째로 다시 요청
        //
        //첫 번째 객체는 현재 사용 중이라 Pool에 대기 객체가 없으므로
        //ComponentPool이 새로운 IG_04 객체를 하나 더 생성하게 됨
        currentIngredient04Second = ingredientRegistry.Get("IG_04", spawnRoot);

        //둘 중 하나라도 가져오지 못했다면 테스트 실패
        if (currentIngredient04 == null ||
            currentIngredient04Second == null)
        {
            Debug.LogWarning("[PoolRegistryTest] IG_04 두 개를 가져오는 데 실패했습니다.");
            return;
        }

        //서로 다른 두 객체가 정상적으로 나왔는지 확인
        Debug.Log(
            $"[PoolRegistryTest] IG_04 두 개 Get 성공 | " +
            $"A: {currentIngredient04.name} / " +
            $"B: {currentIngredient04Second.name}"
            );
    }

    //*현재 사용 중인 IG_04 재료 객체를 Pool로 반환하는 테스트*
    [ContextMenu("IG_04 반환하기")]
    private void ReleaseIngredient04()
    {
        //Registry가 아직 준비되지 않았다면
        //반환 작업을 진행할 수 없으므로 종료
        if (!TryInitializeRegistry()) return;

        //현재 사용 중인 IG_04 객체가 없다면
        //반환할 대상이 없으므로 종료
        if (currentIngredient04 == null)
        {
            Debug.LogWarning("[PoolRegistryTest] 반환할 IG_04 객체가 없습니다.");
            return;
        }

        //현재 IG_04 객체를
        //IG_04 전용 ComponentPool에 반환
        ingredientRegistry.Release("IG_04", currentIngredient04);

        Debug.Log($"[PoolRegistryTest] IG_04 Release 성공: {currentIngredient04.name}");

        //Pool에 반환했으므로
        //현재 사용 중인 객체 참조를 비움
        currentIngredient04 = null;
    }

    //*현재 사용 중인 IG_04 두 개를 모두 Pool로 반환하는 테스트*
    [ContextMenu("IG_04 두 개 반환하기")]
    private void ReleaseTwoIngredient04()
    {
        //Registry가 준비되지 않았다면 테스트 중단
        if (!TryInitializeRegistry()) return;

        //첫 번째 IG_04 객체가 있다면
        //IG_04 전용 Pool로 반환
        if (currentIngredient04 != null)
        {
            ingredientRegistry.Release("IG_04", currentIngredient04);

            currentIngredient04 = null;
        }

        //두 번째 IG_04 객체가 있다면
        //같은 IG_04 전용 Pool로 반환
        if (currentIngredient04Second != null)
        {
            ingredientRegistry.Release("IG_04", currentIngredient04Second);

            currentIngredient04Second = null;
        }

        Debug.Log("[PoolRegistryTest] IG_04 두 개 Release 완료");
    }

    //*잘못된 ItemId로 객체를 반환했을 때
    //Pool 소유권 검사가 정상적으로 막는지 확인하는 테스트*
    [ContextMenu("IG_01을 IG_04로 잘못 반환")]
    private void ReleaseIngredient01WithWrongId()
    {
        //Registry가 준비되지 않았다면 테스트 중단
        if (!TryInitializeRegistry()) return;

        //먼저 IG_01 객체를 가져온 상태여야 함
        if (currentIngredient == null)
        {
            Debug.LogWarning("[PoolRegistryTest] 먼저 IG_01 객체를 가져와주세요.");
            return;
        }

        //실제 객체는 IG_01이지만
        //일부러 잘못된 IG_04 Pool에 반환을 시도
        ingredientRegistry.Release("IG_04", currentIngredient);
    }
}