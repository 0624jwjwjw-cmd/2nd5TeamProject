//**상점과 인벤토리에서 현재 보유 Gold를 표시하는 UI**
using TMPro;
using UnityEngine;

//같은 GameObject에 GoldDisplayUI가 여러 개 붙는 것을 방지
[DisallowMultipleComponent]

public sealed class GoldDisplayUI : MonoBehaviour
{
    //*UI*
    //현재 보유 금액을 표시할 TextMeshPro Text
    [Header("Gold UI")]
    [SerializeField] private TMP_Text goldText;     //보유 금액 표시할 Text

    private CurrencyManager currencyManager;        //실제 Gold 데이터를 가지고 있는 CurrencyManager

    private void OnEnable()
    {
        //이 UI가 켜질 때마다
        //현재 CurrencyManager를 찾아 연결
        BindCurrencyManager();
    }

    private void OnDisable()
    {
        //이 UI가 꺼지면
        //더 이상 Gold 변경 이벤트를 받을 필요가 없으므로 구독 해제
        UnbindCurrencyManager();
    }

    //*CurrencyManager 연결*
    private void BindCurrencyManager()
    {
        //현재 게임에서 사용 중인 CurrencyManager Singleton 가져오기
        currencyManager = CurrencyManager.Instance;

        //CurrencyManager가 아직 없다면 연결할 수 없으므로 종료
        if (currencyManager == null)
        {
            Debug.LogWarning("[GoldDisplayUI] CurrencyManager를 찾지 못했습니다.");
            return;
        }

        //혹시 이전에 같은 이벤트가 등록되어 있다면 먼저 제거
        //중복 구독 방지
        currencyManager.OnRevenueChanged -= RefreshUI;

        //Gold가 변경될 때 RefreshUI가 호출되도록 이벤트 등록
        currencyManager.OnRevenueChanged += RefreshUI;

        //처음 UI를 열었을 때도
        //현재 Gold를 바로 표시해야 하므로 한 번 즉시 갱신
        RefreshUI();
    }

    private void UnbindCurrencyManager()
    {
        //연결된 CurrencyManager가 없다면
        //해제할 이벤트도 없으므로 종료
        if (currencyManager == null) return;

        //Gold 변경 이벤트 구독 해제
        currencyManager.OnRevenueChanged -= RefreshUI;
    }

    //*UI 갱신*
    private void RefreshUI()
    {
        //Inspector에서 GoldText가 연결되지 않았다면 종료
        if (goldText == null)
        {
            Debug.LogWarning("[GoldDisplayUI] Gold Text가 연결되지 않았습니다.");
            return;
        }

        //CurrencyManager가 없다면 표시할 데이터도 없으므로 종료
        if (currencyManager == null) return;

        //예:
        //1000  → 보유 금액 : 1,000 G
        //12500 → 보유 금액 : 12,500 G
        goldText.text = $"보유 금액 : {currencyManager.Gold:N0} G";
    }
}
