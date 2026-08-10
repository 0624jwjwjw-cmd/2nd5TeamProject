using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class RecipeBookUI : MonoBehaviour
{
    [SerializeField] private ReciepeUnlockManager reciepeUnlockManager;
    [SerializeField] private RecipeBookSlotUI recipeBookSlotUI;
    [SerializeField] private RecipeDetailUI recipeDetailUI;
    private void Update()
    {
        OnClickDishIcon();
    }
    private void OnClickDishIcon()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousPos = Mouse.current.position.ReadValue();
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousPos);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider == null) return;

            if (hit.collider.TryGetComponent<DishBase>(out DishBase dishBase))
            {
                if (reciepeUnlockManager.IsUnlocked(dishBase.ID))
                {
                    recipeDetailUI.LockedDish(dishBase);
                }
                else
                {
                    recipeDetailUI.UnlockedDish(dishBase);
                }
            }

        }
    }
}
