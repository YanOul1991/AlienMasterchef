using System.Collections.Generic;

[System.Serializable]
public class Recipe
{
    public EMeal meal;
    public List<Ingredient> requiredIngredients;

    public Recipe(EMeal mealType)
    {
        meal = mealType;
        requiredIngredients = new List<Ingredient>();
    }
}