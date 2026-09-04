using UnityEngine;

public class AddHeart : MonoBehaviour
{
    public void AddHearts()
    {
        SoundManager.Instance.PlaySFX(SFXType.Heart);
        CurrencyManager.Instance.AddHeart(1);
    }
}
