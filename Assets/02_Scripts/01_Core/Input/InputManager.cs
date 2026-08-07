using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public bool IsTap { get; private set; }
    public bool IsDragging { get; private set; }

    public Vector2 PointerPosition { get; private set; }
    public Vector2 DragStartPosition { get; private set; }

    [SerializeField] private float dragThreshold = 10f;

    private bool pointerDown;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        // 매 프레임 초기화
        IsTap = false;

#if UNITY_EDITOR || UNITY_STANDALONE

        HandleMouse();

#else

        HandleTouch();

#endif
    }

    private void HandleMouse()
    {
        PointerPosition = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            pointerDown = true;
            DragStartPosition = PointerPosition;
        }

        if (pointerDown)
        {
            if (Vector2.Distance(DragStartPosition, PointerPosition) > dragThreshold)
            {
                IsDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!IsDragging)
                IsTap = true;

            pointerDown = false;
            IsDragging = false;
        }
    }

    private void HandleTouch()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);
        PointerPosition = touch.position;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                pointerDown = true;
                DragStartPosition = PointerPosition;
                break;

            case TouchPhase.Moved:
                if (Vector2.Distance(DragStartPosition, PointerPosition) > dragThreshold)
                    IsDragging = true;
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:

                if (!IsDragging)
                    IsTap = true;

                pointerDown = false;
                IsDragging = false;
                break;
        }
    }
}