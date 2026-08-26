using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KitchenCookResult : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text dishNameText;
    [SerializeField] private TMP_Text dishInfoText;
    [SerializeField] private TMP_Text donationText;
    [SerializeField] private TMP_Text subscribersText;

    public void SetResultDishInfo(string dishID)
    {
        if(ItemVisualRepository.Instance.TryGetIcon(dishID, out Sprite icon))
        {
            image.sprite = icon;
        }
        else
        {
            return;
        }
        if(GameDataRepository.Instance.TryGetDish(dishID, out DishData dishData))
        {
            dishNameText.text = dishData.DishName;
            dishInfoText.text = dishData.Info;
            donationText.text = dishData.Donation.ToString();
            subscribersText.text = dishData.Subscribers.ToString();
        }
    }
    public void SetResultSpecialDishInfo(string dishID)
    {
        if (ItemVisualRepository.Instance.TryGetIcon(dishID, out Sprite icon))
        {
            image.sprite = icon;
        }
        else
        {
            return;
        }
        if (GameDataRepository.Instance.TryGetSpecialDish(dishID, out DishData dishData))
        {
            dishNameText.text = dishData.DishName;
            dishInfoText.text = dishData.Info;
            donationText.text = dishData.Donation.ToString();
            subscribersText.text = dishData.Subscribers.ToString();
        }
    }
    public void OnClickExitButton()
    {
        gameObject.SetActive(false);
    }
}
