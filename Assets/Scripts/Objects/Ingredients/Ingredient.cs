using UnityEngine;

[System.Serializable]
public class Ingredient
{
    public EBaseIngredient baseIngredient;
    public EIngredientState state;

    [Header("Visual GameObjects")]
    public GameObject originalStateObject;
    public GameObject choppedStateObject;
    public GameObject cookedStateObject;
    public GameObject friedStateObject;
    public GameObject blendedStateObject;

    private EIngredientState previousState;

    public Ingredient(EBaseIngredient ingredient, EIngredientState ingredientState = EIngredientState.Original)
    {
        baseIngredient = ingredient;
        state = ingredientState;
        previousState = ingredientState;
    }

    public void UpdateStateVisuals()
    {
        // Only update if state has changed
        if (state == previousState)
            return;

        // Deactivate all state objects first
        DeactivateAllStateObjects();

        // Activate the appropriate state object
        switch (state)
        {
            case EIngredientState.Original:
                if (originalStateObject != null)
                    originalStateObject.SetActive(true);
                break;
            case EIngredientState.Chopped:
                if (choppedStateObject != null)
                    choppedStateObject.SetActive(true);
                break;
            case EIngredientState.Cooked:
                if (cookedStateObject != null)
                    cookedStateObject.SetActive(true);
                break;
            case EIngredientState.Fried:
                if (friedStateObject != null)
                    friedStateObject.SetActive(true);
                break;
            case EIngredientState.Blended:
                if (blendedStateObject != null)
                    blendedStateObject.SetActive(true);
                break;
        }

        previousState = state;
    }

    private void DeactivateAllStateObjects()
    {
        if (originalStateObject != null)
            originalStateObject.SetActive(false);
        if (choppedStateObject != null)
            choppedStateObject.SetActive(false);
        if (cookedStateObject != null)
            cookedStateObject.SetActive(false);
        if (friedStateObject != null)
            friedStateObject.SetActive(false);
        if (blendedStateObject != null)
            blendedStateObject.SetActive(false);
    }

    public void SetState(EIngredientState newState)
    {
        state = newState;
        UpdateStateVisuals();
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