using UnityEngine;
using System.Collections.Generic;

public class Plate : MonoBehaviour
{
    [SerializeField] private List<Ingredient> ingredientsOnPlate = new List<Ingredient>();

    private void OnTriggerEnter(Collider collision)
    {
        Food food = collision.GetComponent<Food>();
        if (food != null)
        {
            Ingredient ingredient = food.GetIngredient();
            ingredientsOnPlate.Add(ingredient);
            Debug.Log($"Ingredient added to plate: {ingredient}");
            
            // Optionnel : Détruire l'ingrédient après l'avoir ajouté
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