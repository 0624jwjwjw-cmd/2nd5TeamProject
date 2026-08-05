using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BootstrapManager : MonoBehaviour
{
    private List<IInitializable> initializables = new();

    private void Start()
    {
        initializables = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include,FindObjectsSortMode.None)
            .OfType<IInitializable>().OrderBy(x => x.Priority).ToList();

        foreach (IInitializable initializable in initializables)
        {
            Debug.Log($"Initialize : {initializable.GetType().Name}");

            initializable.Initialize();
        }
        Destroy(gameObject);
    }

}
