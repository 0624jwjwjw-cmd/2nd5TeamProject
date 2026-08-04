using UnityEditor;
using UnityEngine;

public class DataImporter : EditorWindow
{
    private string csvFolder = "Assets/02_Scripts/Editor"; //csv 기본 볼더 경로

    //각 csv 파일의 이름들
    private string ingredientCsv = "Ingredient.csv";
    private string dishCsv = "Dish.csv";
    private string specialDishCsv = "SpecialDish.csv";
    private string gradeCsv = "Grade.csv";
    private string reciepePurchaseCsv = "ReciepePurchase.csv";
    private string studioUpgradeCsv = "StudioUpgrade.csv";
    private string kitchenUpgradeCsv = "KitchenUpgrade.csv";

    //에셋이 저장될 폴더 경로
    private string IngredientAssetFolder = "Assets/04_Data/01_Ingredient";
    private string DishAssetFolder = "Assets/04_Data/02_Dish";
    private string GradeAssetFolder = "Assets/04_Data/03_Grade";
    private string ReciepePurchaseAssetFolder = "Assets/04_Data/04_Upgrade";
    private string StudioUpgradeAssetFolder = "Assets/04_Data/04_Upgrade";
    private string KitchenUpgradeAssetFolder = "Assets/04_Data/04_Upgrade";

    private string DishMaterialSeparator = "+";
    private char DishMaterialAmountSeparator = 'x';

    [MenuItem("Tools/GameData/데이터 임포트 창")]
    public static void Open() //메뉴 클릭시 실행
    {
        GetWindow<DataImporter>("Data Importer"); //Data Importer 창 열기
    }
    

}
