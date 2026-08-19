using System;
using UnityEngine;


/*
½´ÆÛ Á¤Àç¿îÀÇ ÈÄ´Ù´Ú ½äÇ®±â!!
Áö±İ À¯Æ©ºêµî±ŞÀº À½½Ä ¸ÔÀ»¶§ ÈÄ¿ø±İ¿¡¸¸ ¹İÀÀÀÌ ¿Í¿ä!
±¸µ¶ÀÚ´Â ½ºÆ©µğ¿À ¾÷±×·¹ÀÌµå·Î ¹èÀ²ÀÌ ´Ş¶óÁ®¿ä!
±×·¡¼­ ÀÌ°Å 2°³ ±¸ºĞÇØ¾ßÇÒ°Å¿¡¿ä!
±×¸®°í ½ÇÇèÇØº¸¸é¼­ ´À³¤°Çµ¥ µ·ÀÌ¶û ±¸µ¶ÀÚ°¡ µé¾î¿À´Â°Ô ¿ø°¡·Î µé¾î¿À´õ¶ó±¸¿©
½Ä»§ ¿ø°¡°¡ 50¿øÀÌ°í ÈÄ¿ø±İ 100¿ø ±¸µ¶ÀÚ 20¸íÀÎµ¥
½Ä»§À» ¸ÔÀ¸¸é ÈÄ¿ø±İÀÌ¶û ±¸µ¶ÀÚ°¡ ¿ø°¡¸¦ ±âÁØÀ¸·Î µé¾î¿Í¿ä
¿¹¸¦ µé¾î¼­ ½Ä»§ 3°³¸¦ ¸ÔÀ¸¸é 150¿ø, 150¸íÀÌ µé¾î¿Í¿ä
¾Æ¸¶ dish.Cost°¡ ¾Æ´Ï¶ó dish.¹¹½Ã±â µû·Î ÀÖÀ»°Å¿¡¿ä
 */

public class LiveManager : MonoBehaviour
{
    public static LiveManager Instance { get; private set; }

    [SerializeField] private float _liveDuration = 20f;

    private float _elapsedTime;
    private bool _isLive;
    private int _lastSecond;

    private int _totalDonation;
    private int _totalSubscribers;

    private int _totalFoodCost;

    public bool IsLive => _isLive;
    public float ElapsedTime => _elapsedTime;
    public int TotalDonation => _totalDonation;
    public int TotalSubscribers => _totalSubscribers;

    public event Action OnLiveStarted;
    public event Action OnLiveStopped;
    public event Action OnLiveEnded;
    public event Action<float> OnLiveTimeChanged;
    public event Action<int> OnDonationChanged;
    public event Action<int> OnSubscribersChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _elapsedTime = 0f;
        _lastSecond = 0;
        _totalDonation = 0;
        _totalSubscribers = 0;
        _totalFoodCost = 0;
    }

    private void Update()
    {
        if (!_isLive)
            return;

        _elapsedTime += Time.deltaTime;

        int currentSecond = Mathf.FloorToInt(_elapsedTime);

        if (currentSecond > _lastSecond)
        {
            _lastSecond = currentSecond;
            OnLiveTimeChanged?.Invoke(currentSecond);
        }

        if (_elapsedTime >= _liveDuration)
            EndLive();
    }

    public void StartLive()
    {
        if (_isLive)
            return;
<<<<<<< HEAD

        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("[LiveManager] CurrencyManager°¡ ¾ø½À´Ï´Ù.");
            return;
        }

        if (!CurrencyManager.Instance.SpendHeart(1))
=======
        if(!CurrencyManager.Instance.SpendHeart(1))//½´ÆÛ Á¤Àç¿îÀÌ Àá½Ã ¸¸Áø°Å¿¡¿ë
>>>>>>> parent of 918d069 (/fix ìŒì‹ ì‹œìŠ¤í…œ ItemId ê¸°ë°˜ìœ¼ë¡œ ë³€ê²½)
        {
            Debug.Log("ÇÏÆ®°¡ ºÎÁ·ÇÕ´Ï´Ù.");
            return;
        }
        _isLive = true;
        _elapsedTime = 0f;
        _lastSecond = 0;

        _totalDonation = 0;
        _totalSubscribers = 0;
        _totalFoodCost = 0;

        Debug.Log("¶óÀÌºê ½ÃÀÛ!");

        OnLiveTimeChanged?.Invoke(0);
        OnLiveStarted?.Invoke();
    }

    // »ç¿ëÀÚ°¡ ¹æ¼Û Áß´Ü ¹öÆ°À» ´­·¶À» ¶§
    public void StopLive()
    {
        if (!_isLive)
            return;

        _isLive = false;

        Debug.Log("¶óÀÌºê Áß´Ü");

        OnLiveStopped?.Invoke();
    }

    // 20ÃÊ°¡ ³¡³µÀ» ¶§
    public void EndLive()
    {
        if (!_isLive)
            return;

        _isLive = false;

        CalculateLiveReward();

        if (GameDateManager.Instance != null)
            GameDateManager.Instance.AddDateCount();

        Debug.Log(
            $"¶óÀÌºê Á¾·á / ÈÄ¿ø±İ: {_totalDonation} / " +
            $"±¸µ¶ÀÚ: {_totalSubscribers}"
        );

        OnLiveEnded?.Invoke();
    }

    public void EatFood(DishBase dish)
    {
<<<<<<< HEAD
        if (!_isLive)
            return;

        if (string.IsNullOrWhiteSpace(itemId))
            return;

        if (GameDataRepository.Instance == null)
        {
            Debug.LogError("[LiveManager] GameDataRepository°¡ ¾ø½À´Ï´Ù.");
            return;
        }
=======
        if (!_isLive || dish == null)
            return;

        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("[LiveManager] CurrencyManager°¡ ¾ø½À´Ï´Ù.");
            return;
        }

        //int youtubeGrade;
>>>>>>> parent of 918d069 (/fix ìŒì‹ ì‹œìŠ¤í…œ ItemId ê¸°ë°˜ìœ¼ë¡œ ë³€ê²½)

        //switch (dish.ReciepeGrade)
        //{
        //    case "±âº»":
        //        youtubeGrade = 0;
        //        break;

<<<<<<< HEAD
        if (!GameDataRepository.Instance.TryGetDish(itemId, out dishData) &&
            !GameDataRepository.Instance.TryGetSpecialDish(itemId, out dishData))
        {
            Debug.LogWarning(
                $"[LiveManager] À½½Ä µ¥ÀÌÅÍ¸¦ Ã£À» ¼ö ¾ø½À´Ï´Ù. ID: {itemId}"
            );
=======
        //    case "ÃÊ±Ş":
        //        youtubeGrade = 1;
        //        break;
>>>>>>> parent of 918d069 (/fix ìŒì‹ ì‹œìŠ¤í…œ ItemId ê¸°ë°˜ìœ¼ë¡œ ë³€ê²½)

        //    case "Áß±Ş":
        //        youtubeGrade = 2;
        //        break;

        //    case "°í±Ş":
        //        youtubeGrade = 3;
        //        break;

        //    default:
        //        Debug.LogError(
        //            $"[LiveManager] ¾Ë ¼ö ¾ø´Â À½½Ä µî±ŞÀÔ´Ï´Ù: {dish.ReciepeGrade}"
        //        );
        //        return;
        //}
        int youtubeGrade = SubscriberRank.Instance.CurrentRank;//½´ÆÛ Á¤Àç¿îÀÌ Àá½Ã ¸¸Áø°Å¿¡¿ë

<<<<<<< HEAD
        _totalFoodCost += dishData.Cost;

        Debug.Log(
            $"À½½Ä ¼·Ãë - {dishData.DishName} / " +
            $"¿ø°¡: {dishData.Cost} / " +
            $"´©Àû ¿ø°¡: {_totalFoodCost}"
        );
    }

    private void CalculateLiveReward()
    {
        if (_totalFoodCost <= 0)
            return;

        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("[LiveManager] CurrencyManager°¡ ¾ø½À´Ï´Ù.");
            return;
        }

        if (SubscriberRank.Instance == null)
        {
            Debug.LogError("[LiveManager] SubscriberRank°¡ ¾ø½À´Ï´Ù.");
            return;
        }

        int youtubeGrade = SubscriberRank.Instance.CurrentRank;
=======
>>>>>>> parent of 918d069 (/fix ìŒì‹ ì‹œìŠ¤í…œ ItemId ê¸°ë°˜ìœ¼ë¡œ ë³€ê²½)

        int beforeGold = CurrencyManager.Instance.Gold;
        int beforeSubscriber = CurrencyManager.Instance.Subscriber;

        CalculateGold.GetDonation(
<<<<<<< HEAD
            _totalFoodCost,
=======
            dish.Cost,
>>>>>>> parent of 918d069 (/fix ìŒì‹ ì‹œìŠ¤í…œ ItemId ê¸°ë°˜ìœ¼ë¡œ ë³€ê²½)
            youtubeGrade
        );

        CalculateSubscriber.GetDonation(
<<<<<<< HEAD
            _totalFoodCost,
=======
            dish.Cost,
>>>>>>> parent of 918d069 (/fix ìŒì‹ ì‹œìŠ¤í…œ ItemId ê¸°ë°˜ìœ¼ë¡œ ë³€ê²½)
            youtubeGrade
        );

        _totalDonation =
            CurrencyManager.Instance.Gold - beforeGold;

        _totalSubscribers =
            CurrencyManager.Instance.Subscriber - beforeSubscriber;

        OnDonationChanged?.Invoke(_totalDonation);
        OnSubscribersChanged?.Invoke(_totalSubscribers);
<<<<<<< HEAD
=======

        Debug.Log(
            $"À½½Ä ¼·Ãë - {dish.DishName} / " +
            $"µî±Ş: {dish.ReciepeGrade} / " +
            $"ÈÄ¿ø±İ +{addedGold} / " +
            $"±¸µ¶ÀÚ +{addedSubscriber}"
        );
>>>>>>> parent of 918d069 (/fix ìŒì‹ ì‹œìŠ¤í…œ ItemId ê¸°ë°˜ìœ¼ë¡œ ë³€ê²½)
    }
}