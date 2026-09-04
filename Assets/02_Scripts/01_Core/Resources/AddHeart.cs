using UnityEngine;

public class AddHeart : MonoBehaviour
{
    public void AddHearts()
    {
        if(CurrencyManager.Instance.Heart >= 10)
        {
            SoundManager.Instance.PlaySFX(SFXType.Lose);
            return;
        }
        SoundManager.Instance.PlaySFX(SFXType.Heart);
        CurrencyManager.Instance.AddHeart(1);
    }
}
