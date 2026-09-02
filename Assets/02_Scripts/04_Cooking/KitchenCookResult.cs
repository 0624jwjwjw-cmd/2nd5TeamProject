using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class KitchenCookResult : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text dishNameText;
    [SerializeField] private TMP_Text dishInfoText;
    [SerializeField] private TMP_Text donationText;
    [SerializeField] private TMP_Text subscribersText;

    public void SetResultDishInfo(string dishID)
    {
        SetData(dishID, false);
    }
    public void SetResultSpecialDishInfo(string dishID)
    {
        SetData(dishID, true);
    }
    private void SetData(string dishID, bool isSpecial)
    {
        if (!ItemVisualRepository.Instance.TryGetIcon(dishID, out Sprite icon))
        {
            return;
        }
        image.sprite = icon;

        DishData dishData;

        if (!isSpecial)
        {
            GameDataRepository.Instance.TryGetDish(dishID, out dishData);
        }
        else
        {
            GameDataRepository.Instance.TryGetSpecialDish(dishID, out dishData);
        }

        dishNameText.text = dishData.DishName;
        dishInfoText.text = dishData.Info;
        donationText.text = dishData.Donation.ToString();
        subscribersText.text = dishData.Subscribers.ToString();
    }
    public void OnClickExitButton()
    {
        gameObject.SetActive(false);
    }
}
