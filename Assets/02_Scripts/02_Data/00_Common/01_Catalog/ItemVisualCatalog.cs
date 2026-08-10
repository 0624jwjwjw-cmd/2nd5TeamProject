//**게임에서 사용하는 재료·일반 요리·특별 요리 프리팹 목록을 보관하는 Catalog**
using System;
using System.Collections.Generic;
using UnityEngine;

//Project 창에서 ItemVisualCatalog 에셋 생성
[CreateAssetMenu(
    fileName = "ItemVisualCatalog",
    menuName = "GameData/Item Visual Catalog"
    )]
public sealed class ItemVisualCatalog : ScriptableObject
{
    //GameObject[]가 아니라 IngredientBase[]로 받는 이유: 
    //재료 프리팹에는 이미 IngredientBase가 붙어 있기 때문에
    //프리팹의 Data에 바로 접근할 수 있음

    //IG_계열 재료 프리팹들을 보관
    [Header("재료 프리팹")]
    [SerializeField] private IngredientBase[] ingredientPrefabs = Array.Empty<IngredientBase>();

    //DS_계열 일반요리 프리팹들을 보관
    [Header("일반 요리 프리팹")]
    [SerializeField] private DishBase[] dishPrefabs = Array.Empty<DishBase>();

    //SD_ 계열의 특별 요리 프리팹들을 보관
    [Header("특별 요리 프리팹")]
    [SerializeField] private DishBase[] specialDishPrefabs = Array.Empty<DishBase>();

    //Prefab 목록 읽기 프로퍼티
    public IReadOnlyList<IngredientBase> IngredientPrefabs => ingredientPrefabs;    //재료 Prefab 목록 읽기
    public IReadOnlyList<DishBase> DishPrefabs => dishPrefabs;                      //일반 요리 Prefab 목록 읽기
    public IReadOnlyList<DishBase> SpecialDishPrefabs => specialDishPrefabs;        //특별 요리 Prefab 목록 읽기
}
