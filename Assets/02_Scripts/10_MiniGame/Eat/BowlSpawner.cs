using System.Collections.Generic;
using UnityEngine;

public class BowlSpawner : MonoBehaviour
{
    [Header("Bowl")]
    [SerializeField] private FoodBowl bowlPrefab;

    [Header("Spawn")]
    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private int bowlCount = 5;

    private List<FoodBowl> bowls = new List<FoodBowl>();

    public void SpawnBowls()
    {
        bowls.Clear();

        for (int i = 0; i < bowlCount; i++)
        {
            FoodBowl bowl = Instantiate(bowlPrefab, spawnArea);

            bool isBowlA = Random.value < 0.5f;
            bowl.Initialize(isBowlA);

            bowls.Add(bowl);
        }
    }

    public List<FoodBowl> GetBowls()
    {
        return bowls;
    }
    public void ClearBowls()
    {
        foreach (FoodBowl bowl in bowls)
        {
            if (bowl != null)
            {
                Destroy(bowl.gameObject);
            }
        }
        bowls.Clear();
    }
}