// ========== OrderManager.cs ==========
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System;

public class OrderManager : NetworkBehaviour
{
  public static OrderManager Singleton;
  [SerializeField] private List<Plat> activeOrders = new();

  private void Awake()
  {
    if (Singleton == null) Singleton = this;
    else Destroy(gameObject);

    for (int i = 0; i < 5; i++) 
      CreateRandomOrder();
  }

  public override void OnDestroy()
  {
    base.OnDestroy();
    Singleton = null;
  }

  public void CreateRandomOrder()
  {
    EMeal _randEMeal = (EMeal)UnityEngine.Random.Range(0, Enum.GetNames(typeof(EMeal)).Length);
    Debug.Log($"[---- OrderManager ----] Generated a new random order of type {_randEMeal}");
    activeOrders.Add(new Plat(_randEMeal));
  }

  public Plat FindMatchingOrder(List<Ingredient> plateIngredients)
  {
    foreach (var order in activeOrders)
    {
      // Créer un plat temporaire avec les ingrédients du plateau
      Plat tempPlat = new(order.mealType);
      foreach (var ingredient in plateIngredients) 
        tempPlat.AddIngredient(ingredient);

      // Vérifier si le plat est complet
      if (tempPlat.IsCompleted()) 
        return order;
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