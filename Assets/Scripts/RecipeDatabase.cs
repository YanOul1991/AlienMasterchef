using System.Collections.Generic;

public static class RecipeDatabase
{
    private static Dictionary<EMeal, Recipe> recipes;

    static RecipeDatabase()
    {
        InitializeRecipes();
    }

    private static void InitializeRecipes()
    {
        recipes = new Dictionary<EMeal, Recipe>();

        // Fish & Chips
        var fishAndChips = new Recipe(EMeal.FishAndChips);
        fishAndChips.requiredIngredients.Add(new Ingredient(EBaseIngredient.Fish, EIngredientState.Fried));
        fishAndChips.requiredIngredients.Add(new Ingredient(EBaseIngredient.Potato, EIngredientState.Fried));
        recipes.Add(EMeal.FishAndChips, fishAndChips);

        // Sushi
        var sushi = new Recipe(EMeal.Sushi);
        sushi.requiredIngredients.Add(new Ingredient(EBaseIngredient.Fish, EIngredientState.Chopped));
        sushi.requiredIngredients.Add(new Ingredient(EBaseIngredient.Rice, EIngredientState.Cooked));
        recipes.Add(EMeal.Sushi, sushi);

        // Onigiri
        var onigiri = new Recipe(EMeal.Onigiri);
        onigiri.requiredIngredients.Add(new Ingredient(EBaseIngredient.Rice, EIngredientState.Cooked));
        recipes.Add(EMeal.Onigiri, onigiri);

        // Bug Nuggets
        var bugNuggets = new Recipe(EMeal.BugNuggets);
        bugNuggets.requiredIngredients.Add(new Ingredient(EBaseIngredient.BugMeat, EIngredientState.Fried));
        recipes.Add(EMeal.BugNuggets, bugNuggets);

        // Meat Mushrooms
        var meatMushrooms = new Recipe(EMeal.MeatMushrooms);
        meatMushrooms.requiredIngredients.Add(new Ingredient(EBaseIngredient.Beef, EIngredientState.Cooked));
        meatMushrooms.requiredIngredients.Add(new Ingredient(EBaseIngredient.Mushroom, EIngredientState.Cooked));
        recipes.Add(EMeal.MeatMushrooms, meatMushrooms);

        // Taco
        var taco = new Recipe(EMeal.Taco);
        taco.requiredIngredients.Add(new Ingredient(EBaseIngredient.Beef, EIngredientState.Cooked));
        taco.requiredIngredients.Add(new Ingredient(EBaseIngredient.Lettuce, EIngredientState.Chopped));
        taco.requiredIngredients.Add(new Ingredient(EBaseIngredient.Tomato, EIngredientState.Chopped));
        recipes.Add(EMeal.Taco, taco);
    }

    public static Recipe GetRecipe(EMeal meal)
    {
        return recipes.ContainsKey(meal) ? recipes[meal] : null;
    }
}