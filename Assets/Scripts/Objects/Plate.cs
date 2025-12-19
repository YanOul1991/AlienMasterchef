using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Plate : MonoBehaviour
{
    [SerializeField] private List<Ingredient> ingredientsOnPlate = new List<Ingredient>();
    [SerializeField] private Transform instantiatePosition1;
    [SerializeField] private Transform instantiatePosition2;
    [SerializeField] private Transform instantiatePosition3;
    [SerializeField] private Transform plate;

    private int currentPositionIndex = 0;

    private void OnTriggerEnter(Collider collision)
    {
        Food food = collision.GetComponent<Food>();
        if (food != null)
        {
            Ingredient ingredient = food.GetIngredient();
            ingredientsOnPlate.Add(ingredient);
            Debug.Log($"Ingredient added to plate: {ingredient}");
            
            // Vérifier si un repas est complet après chaque ajout
            CheckForCompletedMeal();
            
            // Get the current position to use
            Transform currentPosition = GetNextPosition();
            
            // Instantiate the GameObject at the current position
            GameObject newInstance;
            if (currentPosition != null)
            {
                Vector3 spawnPosition = currentPosition.position + new Vector3(0, 0.015f, 0);
                newInstance = Instantiate(collision.gameObject, spawnPosition, currentPosition.rotation);
            }
            else
            {
                newInstance = Instantiate(collision.gameObject, collision.transform.position, collision.transform.rotation);
            }

            Rigidbody rb = newInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Destroy(rb);
            }
            
            newInstance.gameObject.transform.SetParent(plate);
            
            Food foodScript = newInstance.GetComponent<Food>();
            if (foodScript != null)
            {
                Destroy(foodScript);
            }
            
            Destroy(collision.gameObject);
            
            currentPositionIndex++;
        }
    }

    // Nouvelle méthode pour vérifier si un repas est complet
    private void CheckForCompletedMeal()
    {
        EMeal? completedMeal = IsAnyMealComplete();
        
        if (completedMeal.HasValue)
        {
            Debug.Log($"✅ Meal completed on plate: {completedMeal.Value}!");
        }
    }

    // Vérifie si les ingrédients sur l'assiette correspondent à un repas
    public EMeal? IsAnyMealComplete()
    {
        // Parcourir tous les types de repas possibles
        foreach (EMeal meal in System.Enum.GetValues(typeof(EMeal)))
        {
            Recipe recipe = RecipeDatabase.GetRecipe(meal);
            
            if (recipe != null && IsMealComplete(recipe))
            {
                return meal; // Retourne le repas qui est complet
            }
        }
        
        return null; // Aucun repas complet trouvé
    }

    // Vérifie si tous les ingrédients requis d'une recette sont présents
    private bool IsMealComplete(Recipe recipe)
    {
        if (recipe.requiredIngredients.Count == 0)
            return false;

        // Vérifier que tous les ingrédients requis sont présents
        foreach (var required in recipe.requiredIngredients)
        {
            bool found = ingredientsOnPlate.Any(ing => 
                ing.baseIngredient == required.baseIngredient && 
                ing.state == required.state);
            
            if (!found)
                return false;
        }
        
        return true;
    }

    // Méthode publique pour obtenir le repas complet (utile pour OrderStation)
    public EMeal? GetCompletedMeal()
    {
        return IsAnyMealComplete();
    }

    private Transform GetNextPosition()
    {
        switch (currentPositionIndex % 3)
        {
            case 0:
                return instantiatePosition1;
            case 1:
                return instantiatePosition2;
            case 2:
                return instantiatePosition3;
            default:
                return instantiatePosition1;
        }
    }

    public List<Ingredient> GetIngredients()
    {
        return ingredientsOnPlate;
    }

    public void ClearPlate()
    {
        ingredientsOnPlate.Clear();
        currentPositionIndex = 0;
    }
}