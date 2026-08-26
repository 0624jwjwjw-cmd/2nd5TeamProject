using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public bool IsTap { get; private set; }
    public bool IsDragging { get; private set; }
    public bool IsDragReleased { get; private set; }

    public Vector2 PointerPosition { get; private set; }
    public Vector2 DragStartPosition { get; private set; }

    [SerializeField] private float dragThreshold = 10f;

    private bool pointerDown;

    public bool IsPressed => pointerDown;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        IsTap = false;
        IsDragReleased = false;

        HandlePointer();
    }

    private void HandlePointer()
    {
        // =========================
        // 마우스
        // =========================
        if (Mouse.current != null)
        {
            PointerPosition = Mouse.current.position.ReadValue();

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                pointerDown = true;
                DragStartPosition = PointerPosition;
            }

            if (pointerDown)
            {
                if (Vector2.Distance(DragStartPosition,PointerPosition) > dragThreshold)
                {
                    IsDragging = true;
                }
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                if (IsDragging)
                {
                    IsDragReleased = true;
                }
                else
                {
                    IsTap = true;
                }

                pointerDown = false;
                IsDragging = false;
            }
        }

        // =========================
        // 모바일 터치
        // =========================
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            // 터치 위치는 눌려있는 동안 갱신
            if (touch.press.isPressed)
            {
                PointerPosition = touch.position.ReadValue();
            }

            if (touch.press.wasPressedThisFrame)
            {
                pointerDown = true;
                DragStartPosition = PointerPosition;
            }

            if (pointerDown)
            {
                if (Vector2.Distance(DragStartPosition,PointerPosition) > dragThreshold)
                {
                    IsDragging = true;
                }
            }

            if (touch.press.wasReleasedThisFrame)
            {
                if (IsDragging)
                {
                    IsDragReleased = true;
                }
                else
                {
                    IsTap = true;
                }

                pointerDown = false;
                IsDragging = false;
            }
        }
    }
}