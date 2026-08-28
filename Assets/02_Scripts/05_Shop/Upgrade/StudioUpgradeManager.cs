using Unity.VisualScripting;
using UnityEngine;

public class StudioUpgradeManager : MonoBehaviour
{
    [SerializeField] private StudioUpgradeData[] studioUpgradeDatas;

    [SerializeField] private StudioUpgradeData nowData;
    [SerializeField] private StudioUpgradeData nextData;

    private void Awake()
    {
        nowData = studioUpgradeDatas[0];
        nextData = studioUpgradeDatas[1];
    }
    public void LevelUp()
    {
        if(nowData = studioUpgradeDatas[0])
        {
            nowData = studioUpgradeDatas[1];
            nextData = studioUpgradeDatas[2];
        }
        else if(nowData = studioUpgradeDatas[1])
        {
            nowData = studioUpgradeDatas[2];
            nextData = null;
        }
        else
        {
            return;
        }
    }
}
