using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;

public class DataImporter : EditorWindow
{
    //절대 틀리면 안되는 값
    //==============================================================================================
    private string csvFolder = "Assets/02_Scripts/Editor/CSV"; //csv 기본 볼더 경로

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

    private static string DishMaterialSeparator = "+";
    private static char DishMaterialAmountSeparator = 'x';
    //===============================================================================================

    [MenuItem("Tools/GameData/데이터 임포트 창")]
    public static void Open() //메뉴 클릭시 실행
    {
        GetWindow<DataImporter>("Data Importer"); //Data Importer 창 열기
    }

    private void OnGUI() //에디터 창 GUI 그리기
    {
        EditorGUILayout.LabelField("csv 폴더", EditorStyles.boldLabel); //폴더 라벨
        csvFolder = EditorGUILayout.TextField("csv Folder", csvFolder); //CSV폴더 입력 필드

        EditorGUILayout.Space(15); //여백

        EditorGUILayout.LabelField("파일명", EditorStyles.boldLabel); //파일명 라벨
        ingredientCsv = EditorGUILayout.TextField("재료", ingredientCsv);
        dishCsv = EditorGUILayout.TextField("음식", dishCsv);
        specialDishCsv = EditorGUILayout.TextField("특별한 음식", specialDishCsv);
        gradeCsv = EditorGUILayout.TextField("등급", gradeCsv);
        reciepePurchaseCsv = EditorGUILayout.TextField("레시피 구매", reciepePurchaseCsv);
        studioUpgradeCsv = EditorGUILayout.TextField("스튜디오 업그레이드", studioUpgradeCsv);
        kitchenUpgradeCsv = EditorGUILayout.TextField("주방 업그레이드", kitchenUpgradeCsv);

        EditorGUILayout.Space(15);

        //각 버튼 누르면 임포트 함수 실행
        if (GUILayout.Button("0. 모든 SO 삭제", GUILayout.Height(20)))
        { 
            DeletAllData();
        }
        if (GUILayout.Button("1. 재료 임포트", GUILayout.Height(20)))
        {
            ImportIngredients();
        }
        if(GUILayout.Button("2. 음식 임포트", GUILayout.Height(20)))
        {
            ImportDishes();
        }
        if(GUILayout.Button("3. 등급 임포트", GUILayout.Height(20)))
        {
            ImportGrades();
        }
        if(GUILayout.Button("4. 레시피 구매 임포트", GUILayout.Height(20)))
        {
            ImportReciepePurchases();
        }
        if(GUILayout.Button("5. 스튜디오 업그레이트 임포트",GUILayout.Height(20)))
        {
            ImportStudioUpgrades();
        }
        if(GUILayout.Button("6. 주방 업그레이드 임포트",GUILayout.Height(20)))
        {
            ImportKitchenUpgrades();
        }

        EditorGUILayout.Space(15);

        if(GUILayout.Button("전체 임포트", GUILayout.Height(30)))
        {
            ImportIngredients();
            ImportDishes();
            ImportGrades();
            ImportReciepePurchases();
            ImportStudioUpgrades();
            ImportKitchenUpgrades();
        }
    }
    private static T FindOrCreateAsset<T>(string folder, string id) where T : ScriptableObject //에셋을 찾거나 없으면 생성하는 메서드
    {
        EnsureFolder(folder); //폴더가 존재하는지 확인하고 없으면 생성하자
        string path = folder + "/" + id + ".asset"; //에셋 파일 경로 문자열 생성
        T asset = AssetDatabase.LoadAssetAtPath<T>(path); //해당 경로의 에셋을 불러온다.
        if(asset == null) //만약 에셋이 없으면
        {
            asset = ScriptableObject.CreateInstance<T>(); //새로운 SO 인스턴스를 생성한다.
            AssetDatabase.CreateAsset(asset, path); //에셋 파일로 저장한다.
        }
        return asset; //에셋을 반환한다.
    }
    private static void EnsureFolder(string folder) //폴더가 존재하지 않으면 폴더 생성함수
    {
        if (AssetDatabase.IsValidFolder(folder)) return; //해당 폴더가 있으면 리턴
        string[] parts = folder.Split('/'); //폴더 경로를 / 기준으로 분할한다.
        string cur = parts[0]; //첫번째 폴더 이름 저장
        for(int i = 1; i<parts.Length; i++) //나머지 폴더를 다 돌면서
        {
            string next = cur + "/" + parts[i]; //현재 경로에 다음 폴더를 붙여가면서
            if(!AssetDatabase.IsValidFolder(next)) //해당 경로에 폴더가 없다면
            {
                AssetDatabase.CreateFolder(cur, parts[i]); //폴더를 생성한다
            }
            cur = next; //현재 경로를 갱신해준다
        }
    }
    private string CsvPath(string filename) //CSV파일 전체의 경로를 반환한다.
    {
        return Path.Combine(csvFolder, filename); //csvFolder와 파일명을 합쳐서 경롤르 생성한다.
    }
    private static List<T> LoadAllAssets<T>(string folder) where T :ScriptableObject //특정 폴더에서의 모든 에셋을 불러오는 메서드
    {
        List<T> list = new List<T>(); //결과를 저장할 리스트

        if(!AssetDatabase.IsValidFolder(folder)) //폴더가 없으면
        {
            return list; //빈리스트 반환
        }

        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name, new string[] { folder }); //해당 타입의 에셋 GUID를 검색한다
        foreach(string guid in guids) //GUID들을 돌면서
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)); //GUID로 에셋을 불러온다
            if (asset != null) list.Add(asset); //에셋이 존재하면 리스트에 추가한다.
        }

        return list;
    }
    private void ImportIngredients() //재료 csv를 읽어와서 에셋을 생성 및 갱신하는 메서드
    {
        string path = CsvPath(ingredientCsv); //CSV파일 위치 확인
        if (!File.Exists(path)) //파일이 존재하지 않으면
        {
            return; //종료
        }

        List<Dictionary<string, string>> rows = CSVParser.ParseWithHeader(path); //CSV 파싱

        foreach (Dictionary<string, string> row in rows) //각 행을 반복하면서
        {
            string id = CSVParser.Get(row, "ID"); //ID 값을 가져온다
            if (string.IsNullOrWhiteSpace(id)) //ID가 없으면
            {
                continue; //건너뛴다
            }

            IngredientData data = FindOrCreateAsset<IngredientData>(IngredientAssetFolder, id); //에셋을 찾거나 생성한다
            data.SetData(id, CSVParser.Get(row, "이름"), CSVParser.GetInt(row, "가격"), CSVParser.GetInt(row, "후원금"), CSVParser.GetInt(row, "구독자")); //데이터설정
            EditorUtility.SetDirty(data); //에셋 변경된걸 표시해준다.
        }

        AssetDatabase.SaveAssets(); //에셋 저장
        AssetDatabase.Refresh(); //에셋 갱신
    }
    private void ImportDishes() //음식과 특별한음식 CSV읽어와서 Dish
    {
        ImportDishSheet(dishCsv); //일반음식 csv 임포트 하기
        ImportDishSheet(specialDishCsv); //특별한음식 csv 임포트 하기

        AssetDatabase.SaveAssets(); //에셋 저장
        AssetDatabase.Refresh(); //에셋 갱신

    }
    private void ImportDishSheet(string csvFileName) //음식 csv 읽어와서 dishdata 생성
    {
        string path = CsvPath(csvFileName); //csv 파일경로
        if (!File.Exists(path)) //파일이 없으면
        {
            return; //하지말고
        }

        List<Dictionary<string, string>> rows = CSVParser.ParseWithHeader(path); //CSV 파싱

        List<IngredientData> ingredientDatas = LoadAllAssets<IngredientData>(IngredientAssetFolder); //전체 재료데이터 가져오기

        foreach(Dictionary<string, string> row in rows) //행을 돌면서
        {
            string id = CSVParser.Get(row, "ID"); //아이디를 가져오고
            if(string.IsNullOrEmpty(id)) //아이디가없으면
            {
                continue; //건너뛰고
            }

            DishMaterial[] materials = ParseMaterialsByName(CSVParser.Get(row,"레시피"), ingredientDatas); //material의 배열만들기
            DishData data = FindOrCreateAsset<DishData>(DishAssetFolder, id); //데이터 생성
            data.SetData(CSVParser.Get(row, "ID"), CSVParser.Get(row, "이름"), CSVParser.Get(row, "등급"), CSVParser.GetInt(row, "원가"), CSVParser.GetInt(row, "후원금"), CSVParser.GetInt(row, "구독자"), materials, CSVParser.Get(row, "요리설명")); //데이터 넣어주기
            EditorUtility.SetDirty(data); //데이터 갱신알림
        }
    }
    private static DishMaterial[] ParseMaterialsByName(string raw, List<IngredientData> ingredientDatas) //레시피를 찾아서 재료 배열로 만들기
    {
        if (string.IsNullOrWhiteSpace(raw)) //없으면
        {
            return new DishMaterial[0]; //빈 배열 반환
        }

        List<DishMaterial> list = new List<DishMaterial>(); //결과를 저장할 리스트
        string[] entries = raw.Split(new string[] { DishMaterialSeparator }, System.StringSplitOptions.None); //+를 기준으로 분할한다

        foreach (string entry in entries) //행을 반복하며
        {
            string trimmed = entry.Trim(); //앞뒤 공백 제거
            if (trimmed.Length == 0) continue; //빈 항목이면 건너뜀

            int sepIndex = trimmed.LastIndexOf(DishMaterialAmountSeparator); //x의 위치를 찾고
            string name; //재료 이름 텍스트 선언
            string amountStr; //재료 숫자 선언

            if (sepIndex >= 0) //x가 있으면
            {
                name = trimmed.Substring(0, sepIndex).Trim(); //앞부분은 이름
                amountStr = trimmed.Substring(sepIndex + 1).Trim(); //뒷부분은 수량
            }
            else //x가 없으면
            {
                name = trimmed; //전체가 이름
                amountStr = "1"; //기본 수량은 1
            }

            int amount; // 문자열 타입인 숫자를 인트값으로 변환하기위한 변수

            bool isParsed = int.TryParse(amountStr, out amount); //숫자로 변환해보고
            if(!isParsed) //안되면
            {
                amount = 1; //1로
            }

            IngredientData ingredient = null; //재료 데이터 저장용 변수

            foreach (IngredientData i in ingredientDatas) //재료데이터를 돌면서
            {
                if (i.IngredientName == name) //재료데이터에 있는 이름이 있으면
                {
                    ingredient = i; //ingredient에 저장
                    break; //찾았으니 break
                }
            }
            if(ingredient != null) //재료가 있으면
            {
                list.Add(new DishMaterial(ingredient, amount)); //리스트에 저장
            }
            else //없으면
            {
                continue; //건너뛰기
            }

            
            
        }

        return list.ToArray(); // 리스트를 배열로 바꿔서 반환
    }

    private void ImportGrades() //등급 csv를 읽어와서 에셋 생성 및 갱신
    {
        string path = CsvPath(gradeCsv); //csv 파일경로 생성
        if (!File.Exists(path)) //파일이 없으면
        {
            return; //하지말고
        }

        List<Dictionary<string, string>> rows = CSVParser.ParseWithHeader(path); //csv 파싱하고

        foreach (Dictionary<string ,string> row in rows) //각 행을 반복해서
        {
            string id = CSVParser.Get(row, "ID"); //ID값을 먼저 가져오고
            if (string.IsNullOrEmpty(id)) continue; //ID가 없으면 건너뛴다

            GradeData data = FindOrCreateAsset<GradeData>(GradeAssetFolder, id); //에셋을 찾거나 생성하고
            //에셋에 데이터 값들을 넣어준다
            data.SetData(id, CSVParser.Get(row, "이름"), CSVParser.GetInt(row, "구독자"), CSVParser.GetFloat(row, "후원금증가배율"));

            EditorUtility.SetDirty(data); //에셋이 변경되었다고 표시해준다
        }

        AssetDatabase.SaveAssets(); //에셋을 저장하고
        AssetDatabase.Refresh(); //에셋을 갱신한다.
    }
    private void ImportReciepePurchases() // 레시피 구매 csv를 읽어와서 데이터 만들기
    {
        string path = CsvPath(reciepePurchaseCsv); //파일경로 생성
        if (!File.Exists(path)) //파일이 없으면
        {
            return; //종료하고
        }

        List<DishData> dishList = LoadAllAssets<DishData>(DishAssetFolder); //dish데이터를 싹다 가져오고
        Dictionary<string, DishData> dishLookup = new Dictionary<string, DishData>(); // 딕셔너리 생성

        foreach(DishData d in dishList) //각 디쉬데이터를 반복해서
        {
            dishLookup[d.ID] = d; //ID에 Dishdata를 매핑한다
        }

        List<Dictionary<string, string>> rows = CSVParser.ParseWithHeader(path); //CSV 파싱

        foreach(Dictionary<string, string> row in rows) //행을 다 돌면서
        {
            string id = CSVParser.Get(row, "ID"); //ID값을 가져오고
            if(string.IsNullOrEmpty(id)) //ID가 없으면 건너뛰고
            {
                continue;
            }

            ReciepePurchaseData data = FindOrCreateAsset<ReciepePurchaseData>(ReciepePurchaseAssetFolder, id); //데이터를 만들고
            data.SetData(id, CSVParser.Get(row, "이름"), CSVParser.Get(row, "음식ID"), CSVParser.Get(row, "등급"), CSVParser.GetInt(row, "가격"), CSVParser.Get(row, "레시피설명")); //데이터값을 넣어준다

            EditorUtility.SetDirty(data); //에셋이 변경되었다고 표시해주고
        }

        AssetDatabase.SaveAssets(); //에셋 저장
        AssetDatabase.Refresh(); //에셋 갱신
    }
    private void ImportStudioUpgrades() // 스튜디오 업그레이드 CSV를 읽어와서 에셋 생성 및 갱신
    {
        string path = CsvPath(studioUpgradeCsv); //csv 파일 경로
        if(!File.Exists(path)) //파일이 존재하지 않으면
        {
            return; //하지말고
        }

        List<Dictionary<string, string>> rows = CSVParser.ParseWithHeader(path); //csv 파싱

        foreach(Dictionary<string, string> row in rows) //행을 다돌면서
        {
            string id = CSVParser.Get(row, "ID"); //id를 가져오고
            if(string.IsNullOrEmpty(id)) //id가 없으면
            {
                continue; //건너뛰기한다
            }

            StudioUpgradeData data = FindOrCreateAsset<StudioUpgradeData>(StudioUpgradeAssetFolder, id); //생성해주고
            data.SetData(id, CSVParser.GetInt(row, "Lv"), CSVParser.GetInt(row, "가격"), CSVParser.GetFloat(row, "구독자 증가 배율")); //데이터값을 넣어준다

            EditorUtility.SetDirty(data); //변겨표시
        }

        AssetDatabase.SaveAssets(); //에셋 저장
        AssetDatabase.Refresh(); //에셋 갱신
    }
    private void ImportKitchenUpgrades() //주방 업그레이드 csv 읽어와서 에셋 생성 및  갱신
    {
        string path = CsvPath(kitchenUpgradeCsv); //csv파일 경로
        if(!File.Exists(path)) //파일이 없으면
        {
            return; //하지말고
        }

        List<Dictionary<string, string>> rows = CSVParser.ParseWithHeader(path); //csv 파싱하고

        foreach(Dictionary<string, string> row in rows) //행을 다 돌면서
        {
            string id = CSVParser.Get(row, "ID"); //아이디값 가져오기
            if(string.IsNullOrEmpty(id)) //아이디가 없으면
            {
                continue; //건너뛰고
            }

            KitchenUpgradeData data = FindOrCreateAsset<KitchenUpgradeData>(KitchenUpgradeAssetFolder, id); //so생성
            data.SetData(id, CSVParser.GetInt(row, "Lv"), CSVParser.GetInt(row, "가격"), CSVParser.GetInt(row, "특별한음식등장확률")); //데이터값 넣어주기

            EditorUtility.SetDirty(data); //변경표시
        }
        AssetDatabase.SaveAssets(); //저장
        AssetDatabase.Refresh(); //갱신
    }

    private void DeletAllData() //SO 다 삭제하기
    {
        bool confirm = EditorUtility.DisplayDialog("데이터 삭제 경고", "모든 SO 데이터를 삭제하시겠습니까?","예","취소");

        if(!confirm)
        {
            return;
        }

        string[] folders = new string[] //폴더이름 문자열 배열
        {
            IngredientAssetFolder, DishAssetFolder, GradeAssetFolder, ReciepePurchaseAssetFolder,StudioUpgradeAssetFolder,KitchenUpgradeAssetFolder
        };

        foreach(string folder in folders) //폴더들을 다 돌면서
        {
            if (!AssetDatabase.IsValidFolder(folder)) //폴더가 없으면
            {
                continue; //건너뛰고
            }

            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new string[] { folder }); //폴더에서 GUID들을 다 가져와서
            foreach(string guid in guids) //GUID를 돌면서
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);  //guid에 맞는 패스값을 불러오고
                AssetDatabase.DeleteAsset(path); //그 패스에 있는 에셋을 지운다
            }
        }
        AssetDatabase.SaveAssets(); //저장
        AssetDatabase.Refresh(); //갱신
    }


}
