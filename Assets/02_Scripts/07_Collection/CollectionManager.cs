using UnityEngine;
using System.Collections.Generic;

public class CollectionManager : MonoBehaviour
{
    [SerializeField] private List<DishData> collections;
    [SerializeField] private DishData[] dishes;
    [SerializeField] private DishData[] specialDishes;

    [SerializeField] private bool isDS_01Unlocked;
    [SerializeField] private bool isDS_02Unlocked;

}
