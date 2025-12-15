// ========== OrderManager.cs ==========
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class OrderManager : MonoBehaviour
{
    [SerializeField] private List<Plat> activeOrders = new List<Plat>();

    public void CreateOrder(EMeal meal)
    {
        Plat newPlat = new Plat(meal);
        activeOrders.Add(newPlat);
        Debug.Log($"New order created: {meal}");
    }

    public Plat FindMatchingOrder(List<Ingredient> plateIngredients)
    {
        foreach (var order in activeOrders)
        {
            // Créer un plat temporaire avec les ingrédients du plateau
            Plat tempPlat = new Plat(order.mealType);
            foreach (var ingredient in plateIngredients)
            {
                tempPlat.AddIngredient(ingredient);
            }

            // Vérifier si le plat est complet
            if (tempPlat.IsCompleted())
            {
                return order;
            }
        }
        return null;
    }

    public void CompleteOrder(Plat order)
    {
        if (activeOrders.Contains(order))
        {
            Debug.Log($"Order delivered: {order.mealType}");
            activeOrders.Remove(order);
        }
    }

    public List<Plat> GetActiveOrders()
    {
        return activeOrders;
    }
}