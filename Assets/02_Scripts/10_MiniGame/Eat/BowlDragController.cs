using UnityEngine;

public class BowlDragController : MonoBehaviour
{
    [Header("Sink")]
    [SerializeField] private SinkZone sinkA;
    [SerializeField] private SinkZone sinkB;
    [SerializeField] private SinkZoneTween sinkTweenA;
    [SerializeField] private SinkZoneTween sinkTweenB;
    private FoodBowl currentBowl;
    private RectTransform currentBowlRect;
    private Vector3 originalPosition;
    private bool wasDragging;
    private void Update()
    {
        if (currentBowl == null)
        {
            FindEmptyBowl();
        }

        if (currentBowl == null)
        {
            return;
        }
        bool isDragging =InputManager.Instance.IsPressed &&InputManager.Instance.IsDragging;
        if (isDragging && !wasDragging)
        {
            SoundManager.Instance.PlaySFX(SFXType.MBowl);
        }

        if (isDragging)
        {
            currentBowlRect.position = InputManager.Instance.PointerPosition;
            CheckSinkHighlight();
        }
        if (InputManager.Instance.IsDragReleased)
        {
            CheckSink();
        }
        wasDragging = isDragging;
    }

    private void FindEmptyBowl()
    {
        FoodBowl[] bowls = GetComponentsInChildren<FoodBowl>();

        foreach (FoodBowl bowl in bowls)
        {
            if (!bowl.IsEmpty)
            {
                continue;
            }
            currentBowl = bowl;
            currentBowlRect = bowl.GetComponent<RectTransform>();
            originalPosition = currentBowlRect.position;
            break;
        }
    }
    private void CheckSink()
    {
        Vector2 position = InputManager.Instance.PointerPosition;
        sinkTweenA.SetHighlight(false);
        sinkTweenB.SetHighlight(false);
        if (sinkA.IsInside(position))
        {
            sinkA.CheckBowl(currentBowl); 
        }
        else if (sinkB.IsInside(position))
        {
            sinkB.CheckBowl(currentBowl); 
        }
        else
        {
            currentBowlRect.position = originalPosition;
        }
        currentBowl = null;
        currentBowlRect = null;
        wasDragging = false;
    }
    private void CheckSinkHighlight()
    {
        Vector2 position = InputManager.Instance.PointerPosition;

        bool isInsideA = sinkA.IsInside(position);
        bool isInsideB = sinkB.IsInside(position);

        sinkTweenA.SetHighlight(isInsideA);
        sinkTweenB.SetHighlight(isInsideB);
    }
}
