using UnityEngine;

public class CoinTextDestroy : MonoBehaviour
{
    [SerializeField] private float destroyTime = 2f;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}
