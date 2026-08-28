//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class CookResult : MonoBehaviour
//{
//    [SerializeField] private Image image;
//    [SerializeField] private TMP_Text dishNameText;
//    [SerializeField] private TMP_Text dishInfoText;
//    [SerializeField] private TMP_Text donationText;
//    [SerializeField] private TMP_Text subscribersText;

//    public void SetResultInfo(DishBase dishBase)
//    {
//        image.sprite = dishBase.spriteRenderer.sprite;
//        dishNameText.text = dishBase.DishName;
//        dishInfoText.text = dishBase.Info;
//        donationText.text = dishBase.Donation.ToString();
//        subscribersText.text = dishBase.Subscribers.ToString();
//    }
//    public void OnClickExitButton()
//    {
//        gameObject.SetActive(false);
//    }
//}
