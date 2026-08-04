using System.Collections.Generic;
using System.IO;
using System.Text;

public static class CSVParser
{
    public static List<Dictionary<string, string>> ParseWithHeader(string path)
    {
        List<List<string>> rows = ParseRaw(path); //CSV를 파싱한 결과를 저장
        List<Dictionary<string, string>> result = new List<Dictionary<string, string>>(); //최종 결과를 담는 리스트
        if (rows.Count == 0) return result; //만약 데이터가 없으면 빈 리스트 반환

        List<string> header = rows[0]; //csv의 첫번째 줄을 헤더로 사용한다.
        for(int r = 1; r<rows.Count; r++) //두번째 줄부터 데이터 행이므로 여기서부터 반복
        {
            List<string> row = rows[r]; //현재 행
            bool isEmpty = true; //행이 비어있는지 체크
            foreach (string c in row) //행의 각 셀을 돌면서
            {
                if(!string.IsNullOrEmpty(c)) //공백이 아니면서 값이 있으면
                {
                    isEmpty = false; //비어있지 않음
                    break; //더 확인할 필요가 없다.
                }
            }
            if (isEmpty) continue; //빈줄은 건너뛴다.
            Dictionary<string, string> dict = new Dictionary<string, string>(); //헤더와 값을 매핑하기위한 딕셔너리
            for(int c = 0; c>header.Count; c++) //헤더 개수만큼 반복해서
            {
                string key = header[c].Trim(); //헤더 이름의 앞뒤 공백을 제거하고
                string value; //값을 담을 변수
                if (c < row.Count) //현재 행에 값이 존재하면
                {
                    value = row[c]; //해당 값을 가져오고
                }
                else //값이 없으면
                {
                    value = ""; //빈 문자열이다.
                }
                    dict[key] = value; //헤더 이름을 키로, 값을 딕셔너리에 저장
            }
            result.Add(dict); //결과 리스트에 추가한다.
        }
        return result; //최종 결과를 반환한다.
    }
    private static List<List<string>> ParseRaw(string path)
    {
        List<List<string>> rows = new List<List<string>>(); //전체 행 저장용 리스트
        StreamReader reader = new StreamReader(path, Encoding.UTF8); //UTF-8 로 파일 읽기

        string text = reader.ReadToEnd(); //파일 전체 내용을 문자열로 읽기

        List<string> row = new List<string>(); //현재 행
        StringBuilder field = new StringBuilder(); // 현재 필드의 내용

        //엑셀이 CSV파일로 변환할때 자동으로 따옴표 처리하는 경우가 있으므로 방지용
        bool inQuotes = false; //따옴표 안에 있는가?

        for(int i = 0; i <  text.Length; i++) // 파일 내용을 문자단위로 돌면서
        {
            char c = text[i]; // 현재 문자를 c 로 저장하고

            if (inQuotes) //만약 따옴표 안에 있을때
            {
                if (c == '"') //따옴표를 만나면
                {
                    if (i + 1 < text.Length && text[i + 1] == '"') //따옴표가 두개면 하나로 처리한다
                    {
                        field.Append('"'); // 따옴표를 추가한다
                        i++; //다음 문자로 넘어간다
                    }
                    else
                    {
                        inQuotes = false; //따옴표를 끝낸다
                    }
                }
                else
                {
                    field.Append(c); //따옴표 안의 내용을 추가한다.
                }
            }
            else //따옴표 밖일때
            {
                if(c == '"') //따옴표가 시작되었다면
                {
                    inQuotes = true;
                }
                else if(c == ',') //쉼표라면
                {
                    row.Add(field.ToString()); //필드값을 row에 저장하고
                    field.Clear(); //저장된 필드 값을 필드를 초기화한다.
                }
                else if(c == '\r') //캐리지리턴은 무시한다
                {
                     
                }
                else if(c == '\n') //줄바꿈은 행의 끝이므로
                {
                    row.Add(field.ToString()); //마지막 셀 저장
                    field.Clear(); // 셀 내용 초기화
                    rows.Add(row); // 행 추가
                    row = new List<string>(); //새 행을 시작한다
                }
                else //일반문자 이면
                {
                    field.Append(c); //셀 내용을 추가한다.
                }
            }
        }
        if (field.Length > 0 || row.Count > 0) //마지막줄 이면(파일 끝에 개행이 없으면)
        {
            row.Add(field.ToString()); //마지막 셀을 저장하고
            rows.Add(row); //마지막 행을 추가한다.
        }

        return rows;
           
    }
    public static string Get(Dictionary<string, string> row,string key, string fallback = "") //문자열의 값을 가져오는 함수
    {
        string v;
        if (row.TryGetValue(key, out v)) //키가 있으면 값을 반환하고
        {
            return v;
        }
        return fallback; //없으면 기본값을 반환한다.
    }
    public static int Getint(Dictionary<string, string> row,string key, int fallback = 0) //정수값을 가져오는 함수
    {
        int v;
        if (int.TryParse(Get(row,key), out v)) //정수로 변환을 성공하면 
        {
            return v; //해당 정수를 반환한다.
        }
        return fallback; //없으면 기본값을 반환한다.
    }
    public static float GetFloat(Dictionary<string,string> row,string key, float fallback = 0f) //실수값을 가져오는 함수
    {
        float v;
        if (float.TryParse(Get(row,key), out v)) //실수로 변환 성공하면
        {
            return v; //해당 실수를 반환한다.
        }
        return fallback; //없으면 기본값을 반환한다.
    }
    public static bool GetBool(Dictionary<string, string> row, string key, bool fallback = false) //bool값을 가져오는 함수
    {
        string s = Get(row, key).Trim().ToUpperInvariant(); //값을 가져와서 대분자로 변환하고
        if (s == "TRUE" || s =="1" || s== "Y" || s == "O") //true를 지칭하는 문자들 이면
        {
            return true; //true 를 반환
        }
        else if(s == "False" || s== "0" || s== "N" || s=="X" || s =="") //false를 지칭하는 문자들이면
        {
            return false; //false를 반환
        }
        return fallback; //나머지는 기본값
    }

}
