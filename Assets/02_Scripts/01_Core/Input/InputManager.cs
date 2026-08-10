using UnityEngine;
using UnityEngine.InputSystem;

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
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        // 매 프레임 초기화
        IsTap = false;
        HandlePointer();
    }

    private void HandlePointer()
    {
        // 마우스 입력
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
                if (Vector2.Distance(DragStartPosition, PointerPosition) > dragThreshold)
                {
                    IsDragging = true;
                }
            }
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                if (!IsDragging)
                    IsTap = true;

                pointerDown = false;
                IsDragging = false;
            }
        }

        // 모바일 터치 입력
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (!touch.press.isPressed) return;
            PointerPosition = touch.position.ReadValue();
            if (touch.press.wasPressedThisFrame)
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
            if (touch.press.wasReleasedThisFrame)
            {
                if (!IsDragging)
                    IsTap = true;

                pointerDown = false;
                IsDragging = false;
            }
        }
    }
}