using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Plat
{
    public EMeal mealType;
    public List<Ingredient> ingredients;
    public bool completed = false;

    private Recipe recipe;

    public Plat(EMeal meal)
    {
        mealType = meal;
        ingredients = new List<Ingredient>();
        recipe = RecipeDatabase.GetRecipe(meal);
    }

    public void AddIngredient(Ingredient ingredient)
    {
        ingredients.Add(ingredient);
        CheckCompletion();
    }

    public bool IsCompleted()
    {
        return completed;
    }

    private void CheckCompletion()
    {
        if (recipe == null || recipe.requiredIngredients.Count == 0)
        {
            completed = false;
            return;
        }

        // Vérifier si tous les ingrédients requis sont présents
        foreach (var required in recipe.requiredIngredients)
        {
            bool found = ingredients.Any(ing => 
                ing.baseIngredient == required.baseIngredient && 
                ing.state == required.state);
            
            if (!found)
            {
                completed = false;
                return;
            }
        }

        completed = true;
    }

    public float GetCompletionPercentage()
    {
        if (recipe == null || recipe.requiredIngredients.Count == 0)
            return 0f;

        int matchedIngredients = 0;
        foreach (var required in recipe.requiredIngredients)
        {
            if (ingredients.Any(ing => 
                ing.baseIngredient == required.baseIngredient && 
                ing.state == required.state))
            {
                matchedIngredients++;
            }
        }

        return (float)matchedIngredients / recipe.requiredIngredients.Count;
    }

    public override string ToString()
    {
        return $"{mealType} - Completed: {completed} ({GetCompletionPercentage() * 100}%)";
    }
}