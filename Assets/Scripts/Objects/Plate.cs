using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;

public class Plate : NetworkBehaviour
{
    [SerializeField] private List<Ingredient> ingredientsOnPlate = new List<Ingredient>();
    [SerializeField] private Transform instantiatePosition1;
    [SerializeField] private Transform instantiatePosition2;
    [SerializeField] private Transform instantiatePosition3;
    [SerializeField] private Transform plate;

    private int currentPositionIndex = 0;

    private void OnTriggerEnter(Collider collision)
    {
        if (!IsServer) return;

        if (collision.TryGetComponent(out Food _food))
        {
            GameObject _targetObject = collision.gameObject;

            Ingredient ingredient = _food.GetIngredient()   ;
            ingredientsOnPlate.Add(ingredient);

            CheckForCompletedMeal();

            Transform currentPosition = GetNextPosition();
            if (currentPosition != null)
                _targetObject.transform.SetPositionAndRotation(currentPosition.position + (Vector3.up * 0.015f), currentPosition.rotation);
            else
                _targetObject.transform.SetPositionAndRotation(collision.transform.position, collision.transform.rotation);

            _targetObject.transform.SetParent(plate);

            if (_targetObject.TryGetComponent(out Rigidbody rb)) 
                Destroy(rb);

            Collider[] colliders = _targetObject.GetComponentsInChildren<Collider>();

            foreach (Collider collider in colliders) 
                Destroy(collider);

            Destroy(_food);
            
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