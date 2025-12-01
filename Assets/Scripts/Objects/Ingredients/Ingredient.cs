using UnityEngine;

[System.Serializable]
public class Ingredient
{
    public EBaseIngredient baseIngredient;
    public EIngredientState state;

    public Ingredient(EBaseIngredient ingredient, EIngredientState ingredientState = EIngredientState.Original)
    {
        baseIngredient = ingredient;
        state = ingredientState;
    }

    public bool Matches(EBaseIngredient ing, EIngredientState st)
    {
        return baseIngredient == ing && state == st;
    }

    public override string ToString()
    {
        return $"{state} {baseIngredient}";
    }
}