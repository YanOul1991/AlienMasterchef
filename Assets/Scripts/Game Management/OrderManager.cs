// ========== OrderManager.cs ==========
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(NetworkObject))]
public class OrderManager : NetworkBehaviour
{
  public static OrderManager Singleton;
  [SerializeField] private List<Plat> activeOrders = new();
  private List<EMeal> m_validMeals = new();

#if UNITY_EDITOR
  [field: SerializeField] public bool _debugFixedOrder;
#endif

  private void Awake()
  {
    if (Singleton == null) Singleton = this;
    else Destroy(gameObject);
  }

  public void Start()
  {
    NetworkServer.OnGameStarted += delegate () 
    {

      m_validMeals = new();

      for (int i = 0; i < Enum.GetNames(typeof(EMeal)).Length; i++) 
        m_validMeals.Add((EMeal)i);

      CreateRandomOrder();
      CreateRandomOrder();
      CreateRandomOrder();

      Debug.Log("[----- OrderManager -----] GameHasStarted");
    };
  }

  public override void OnDestroy()
  {
    base.OnDestroy();
    Singleton = null;
  }

  public void CreateRandomOrder()
  {
    if (!IsHost) return;
#if UNITY_EDITOR
    Debug.Log("[----- OrderManager -----] CREATING A NEW ORDER");
    if (_debugFixedOrder)
    {
      EMeal _rand = EMeal.MeatMushrooms;
      activeOrders.Add(new Plat(_rand));
      OrderGenerator.Singleton.GenerateTicketRpc(_rand);
    }
#endif
    // EMeal _randEMeal = (EMeal)UnityEngine.Random.Range(0, Enum.GetNames(typeof(EMeal)).Length);
    EMeal _randMeal = m_validMeals[UnityEngine.Random.Range(0, m_validMeals.Count)];
    activeOrders.Add(new Plat(_randMeal));
    OrderGenerator.Singleton.GenerateTicketRpc(_randMeal);
    m_validMeals.Remove(_randMeal);
  }

  public bool CheckOrder(EMeal _meal)
  {
#if UNITY_EDITOR
    Debug.Log($"[---- OrderManager ---] Checking for order {_meal}");
#endif
    foreach (Plat order in activeOrders)
    {
      if (order.mealType == _meal)
      {
#if UNITY_EDITOR
        Debug.Log($"[---- OrderManager ---] Following order is completed {_meal}");
#endif
        activeOrders.Remove(order);
        m_validMeals.Add(_meal);
        return true;
      }
    }
    return false;
  }
}