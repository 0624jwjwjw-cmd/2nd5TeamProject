using UnityEngine;

public class BowlDragController : MonoBehaviour
{
    [Header("Sink")]
    [SerializeField] private SinkZone sinkA;
    [SerializeField] private SinkZone sinkB;

    private FoodBowl currentBowl;
    private RectTransform currentBowlRect;
    private Vector3 originalPosition;
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

        if (InputManager.Instance.IsPressed &&InputManager.Instance.IsDragging)
        {
            currentBowlRect.position =InputManager.Instance.PointerPosition;
        }
        if (InputManager.Instance.IsDragReleased)
        {
            CheckSink();
        }
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
    }
}
