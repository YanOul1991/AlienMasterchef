// using UnityEngine;

// public class OrderStation : MonoBehaviour
// {
//   [SerializeField] private OrderManager orderManager;

//   private void OnTriggerEnter(Collider other)
//   {
//     Plate plate = other.GetComponent<Plate>();
//     if (plate != null)
//     {
//       VerifyOrder(plate);
//     }
//   }

//   private void VerifyOrder(Plate plate)
//   {
//     var plateIngredients = plate.GetIngredients();

//     if (plateIngredients.Count == 0)
//     {
//       Debug.Log("Empty plate! No ingredients to verify.");
//       return;
//     }

//     // Vérifier si le plat correspond à une commande active
//     Plat matchingOrder = orderManager.FindMatchingOrder(plateIngredients);

//     if (matchingOrder != null)
//     {
//       Debug.Log($"Order completed: {matchingOrder.mealType}!");
//       orderManager.CompleteOrder(matchingOrder);
//       plate.ClearPlate();
//     }
//     else
//     {
//       Debug.Log("No matching order found or order incomplete!");
//     }
//   }
// }