//**ItemId를 기준으로 여러 종류의 Prefab Pool을 관리하기 위한 공용 Registry**
using System;
using System.Collections.Generic;
using UnityEngine;

//ComponentPool<T>
//→ 하나의 Prefab을 생성 / 재사용 / 반환하는 역할
//
//ItemComponentPoolRegistry<T>
//→ ItemId를 기준으로
//   어떤 ComponentPool<T>를 사용할지 선택하는 역할
//
//사용 예:
//IG_01 → 빵 Prefab Pool
//DS_01 → 일반 요리 Prefab Pool
//SD_01 → 특별 요리 Prefab Pool

//T는 Unity Component를 상속받는 타입만 사용할 수 있음
//
//예:
//ItemComponentPoolRegistry<IngredientBase>
//ItemComponentPoolRegistry<DishBase>
public sealed class ItemComponentPoolRegistry<T> where T : Component
{
    //ItemId를 이용해 실제 Prefab을 찾기 위한 Repository
    private readonly IItemVisualRepository itemVisualRepository;


    //사용하지 않는 객체들을 보관할 공통 부모
    private readonly Transform poolRoot;

    //ItemId → 해당 Item 전용 ComponentPool
    private readonly Dictionary<string, ComponentPool<T>> pools = new Dictionary<string, ComponentPool<T>>(StringComparer.Ordinal);

    //*ID 기반 Pool Registry 생성*
    public ItemComponentPoolRegistry(
        IItemVisualRepository itemVisualRepository,
        Transform poolRoot)
    {
        //Repository가 없다면
        //ItemId로 Prefab을 검색할 수 없음
        if (itemVisualRepository == null)
        {
            throw new ArgumentNullException(nameof(itemVisualRepository));
        }

        //Pool 객체를 보관할 부모가 없다면
        //정상적인 Pool을 만들 수 없음
        if (poolRoot == null)
        {
            throw new ArgumentNullException(nameof(poolRoot));
        }

        //외부에서 전달받은 의존성 저장
        this.itemVisualRepository = itemVisualRepository;
        this.poolRoot = poolRoot;
    }

    //*ItemId에 해당하는 객체를 Pool에서 가져오기*
    public T Get(string itemId, Transform parent = null)
    {
        //ItemId에 해당하는 Pool을 가져오거나
        //아직 없다면 새로 생성
        if (!TryGetOrCreatePool(itemId, out ComponentPool<T> pool))
        {
            //Prefab을 찾지 못했거나
            //Prefab에 T 컴포넌트가 없는 경우 실패
            return null;
        }

        //찾아낸 ItemId 전용 Pool에서
        //실제로 사용할 객체를 가져옴
        //
        //parent가 전달됐다면
        //ComponentPool.Get() 내부에서 해당 부모 아래로 이동
        return pool.Get(parent);
    }

    //*사용이 끝난 객체를 ItemId에 해당하는 Pool로 반환*
    public void Release(string itemId, T targetObject)
    {
        //반환할 객체가 없다면 종료
        if (targetObject == null) return;

        //ItemId가 비어 있다면 어느 Pool로 반환해야 하는지 알 수 없으므로 실패
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning(
                $"[ItemComponentPoolRegistry<{typeof(T).Name}>] " +
                $"Release할 ItemId가 비어 있습니다."
                );

            return;
        }

        //Release할 때는 새로운 Pool을 만들지 않음
        //
        //정상적으로 Get()으로 가져온 객체라면
        //해당 ItemId의 Pool은 이미 Dictionary에 존재해야 함
        if (!pools.TryGetValue(
            itemId,
            out ComponentPool<T> pool))
        {
            Debug.LogWarning(
                $"[ItemComponentPoolRegistry<{typeof(T).Name}>] " +
                $"반환할 Pool을 찾을 수 없습니다: {itemId}"
                );

            return;
        }

        //실제 객체 반환은 해당 ItemId의 ComponentPool이 담당
        //
        //ComponentPool 내부에서
        //이 Pool이 직접 생성한 객체인지도 다시 검사함
        pool.Release(targetObject);
    }

    //*ItemId에 해당하는 Pool을 찾거나 없으면 새로 생성*
    private bool TryGetOrCreatePool(string itemId, out ComponentPool<T> pool)
    {
        //실패했을 때 반환할 기본값
        pool = null;

        //ItemId가 비어 있다면 어떤 Prefab을 찾아야 하는지 알 수 없으므로 실패
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning(
                $"[ItemComponentPoolRegistry<{typeof(T).Name}>] " +
                $"ItemId가 비어 있습니다."
                );

            return false;
        }

        //이미 이 ItemId 전용 Pool을 만든 적이 있다면
        //새로 만들지 않고 기존 Pool을 바로 재사용
        if (pools.TryGetValue(itemId, out pool))
        {
            return true;
        }

        //아직 Pool이 없다면
        //ItemVisualRepository에서 ItemId에 해당하는 Prefab 검색
        if (!itemVisualRepository.TryGetPrefab(itemId, out GameObject prefab))
        {
            Debug.LogWarning(
                $"[ItemComponentPoolRegistry<{typeof(T).Name}>] " +
                $"Prefab을 찾을 수 없습니다: {itemId}"
                );

            return false;
        }

        //찾아온 GameObject Prefab에
        //현재 Registry가 요구하는 T 컴포넌트가 있는지 확인
        //
        //예:
        //ItemComponentPoolRegistry<IngredientBase>라면
        //Prefab에 IngredientBase가 있어야 함
        if (!prefab.TryGetComponent(out T componentPrefab))
        {
            Debug.LogWarning(
                $"[ItemComponentPoolRegistry<{typeof(T).Name}>] " +
                $"Prefab에 {typeof(T).Name} 컴포넌트가 없습니다: {itemId}"
                );

            return false;
        }

        //해당 ItemId 전용 ComponentPool 생성
        pool = new ComponentPool<T>(componentPrefab, poolRoot);

        //다음에 같은 ItemId가 요청되면
        //다시 Prefab을 검색하거나 Pool을 생성하지 않도록 Dictionary에 저장
        pools.Add(itemId, pool);

        return true;
    }
}

/*
예)
빵 가져오기
IngredientBase bread = ingredientPoolRegistry.Get("IG_01", cookingArea);

사용이 끝난 빵 반환
ingredientPoolRegistry.Release("IG_01", bread);
*/