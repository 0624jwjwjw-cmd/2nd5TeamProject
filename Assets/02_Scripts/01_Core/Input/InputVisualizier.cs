using UnityEngine;

public class InputVisualizer : MonoBehaviour
{
    [Header("Tap Effect")]
    [SerializeField] private GameObject tapEffectPrefab;
    [SerializeField] private float tapLifeTime = 0.3f;

    [Header("Drag Line")]
    [SerializeField] private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        ShowTap();
        ShowDrag();
    }


    private void ShowTap()
    {
        if (InputManager.Instance.IsTap)
        {
            Vector3 pos = ScreenToWorld(InputManager.Instance.PointerPosition);
            GameObject effect = Instantiate(tapEffectPrefab,pos,Quaternion.identity);
            Destroy(effect, tapLifeTime);
        }
    }


    private void ShowDrag()
    {
        if (InputManager.Instance.IsDragging)
        {
            lineRenderer.enabled = true;
            Vector3 start = ScreenToWorld(InputManager.Instance.DragStartPosition);
            Vector3 current = ScreenToWorld(InputManager.Instance.PointerPosition);
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, current);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }


    private Vector3 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 worldPos =Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
        return worldPos;
    }
}