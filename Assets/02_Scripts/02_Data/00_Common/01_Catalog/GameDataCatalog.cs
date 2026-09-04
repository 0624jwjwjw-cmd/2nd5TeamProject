//**재료ㆍ일반 요리ㆍ특별 요리 데이터 목록을 보관하는 카탈로그**
//
//역할: 데이터 목록 보관만 담당
using System;
using System.Collections.Generic;   //리스트 쓸거얌
using UnityEngine;

//Project창의 Create 메뉴에서 GameDataCatalog 에셋을 생성할 수 있게 함
[CreateAssetMenu(
    fileName = "GameDataCatalog", 
    menuName = "GameData/Game Data Catalog"
    )]
public sealed class GameDataCatalog : ScriptableObject
{
    [Header("재료 데이터")]
    //CSV 임포터로 생성된 IngredientData 에셋들을 보관
    //
    //재료 데이터는 게임 도중 추가되거나 삭제되는 데이터가 아니라
    //개발 단계에서 미리 정해지는 고정 데이터이므로 크기가 고정된 배열을 사용
    //
    //Array.Empty<IngredientData>()를 사용하면 배열이 null인 상태로 시작하는 것을 방지 가능
    [SerializeField] private IngredientData[] ingredients = Array.Empty<IngredientData>();

    [Header("일반 요리 데이터")]
    //DS_ ID를 사용하는 일반 요리 데이터 보관
    [SerializeField] private DishData[] dishes = Array.Empty<DishData>();

    [Header("특별 요리 데이터")]
    //SD_ ID를 사용하는 특별 요리 데이터 보관
    [SerializeField] private DishData[] specialDishes = Array.Empty<DishData>();

    //프로퍼티
    public IReadOnlyList<IngredientData> Ingredients => ingredients;    //재료
    public IReadOnlyList<DishData> Dishes => dishes;                    //일반 요리
    public IReadOnlyList<DishData> SpecialDishes => specialDishes;      //특별 요리
}
