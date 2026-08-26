using UnityEngine;
using UnityEngine.UI;

public class FoodPlayerVisual : MonoBehaviour
{
    [Header("Player Image")]
    [SerializeField] private Image playerImage;

    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite eatSprite1;
    [SerializeField] private Sprite eatSprite2;

    [Header("Setting")]
    [SerializeField] private float idleTime = 1f;

    private bool useFirstEatSprite = true;
    private float idleTimer;

    private void Update()
    {
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleTime)
        {
            SetIdle();
        }
    }

    public void PlayEat()
    {
        idleTimer = 0f;

        if (useFirstEatSprite)
        {
            playerImage.sprite = eatSprite1;
        }
        else
        {
            playerImage.sprite = eatSprite2;
        }

        useFirstEatSprite = !useFirstEatSprite;
    }

    private void SetIdle()
    {
        playerImage.sprite = idleSprite;
    }
}