using UnityEngine;
using System.Collections.Generic;

public class Plate : MonoBehaviour
{
    [SerializeField] private List<Ingredient> ingredientsOnPlate = new List<Ingredient>();
    [SerializeField] private Transform instantiatePosition;
    [SerializeField] private Transform plate;

    private void OnTriggerEnter(Collider collision)
    {
        Food food = collision.GetComponent<Food>();
        if (food != null)
        {
            Ingredient ingredient = food.GetIngredient();
            ingredientsOnPlate.Add(ingredient);
            Debug.Log($"Ingredient added to plate: {ingredient}");
            
            // Instantiate the GameObject at the specified position
            GameObject newInstance;
            if (instantiatePosition != null)
            {
                Vector3 spawnPosition = instantiatePosition.position + new Vector3(0, 0.015f, 0);
                newInstance = Instantiate(collision.gameObject, spawnPosition, instantiatePosition.rotation);
            }
            else
            {
                // Fallback: instantiate at the original position if no Transform is assigned
                newInstance = Instantiate(collision.gameObject, collision.transform.position, collision.transform.rotation);
            }

            Rigidbody rb = newInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Destroy(rb);
            }
            
            // Set the plate as the parent of the new instance
            newInstance.gameObject.transform.SetParent(plate);
            
            // Remove the Food script from the new instance
            Food foodScript = newInstance.GetComponent<Food>();
            if (foodScript != null)
            {
                Destroy(foodScript);
            }
            
            // Destroy the original GameObject
            Destroy(collision.gameObject);
        }
    }

    public List<Ingredient> GetIngredients()
    {
        return ingredientsOnPlate;
    }

    public void ClearPlate()
    {
        ingredientsOnPlate.Clear();
    }
}