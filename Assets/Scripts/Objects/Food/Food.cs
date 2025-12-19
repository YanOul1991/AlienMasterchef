using UnityEngine;

public class Food : MonoBehaviour, IInteractionListener
{
	[SerializeField] private FoodStateTransform[] foodStates;
	[SerializeField] private Ingredient currentIngredient;

	private void Start()
	{
		if (currentIngredient == null)
		{
			currentIngredient = new Ingredient(EBaseIngredient.Fish, EIngredientState.Original);
		}
	}

	public Ingredient GetIngredient()
	{
		return currentIngredient;
	}

	public void SetIngredient(Ingredient ingredient)
	{
		currentIngredient = ingredient;
	}

	public void InteractionListen(IInteractionTriggerer triggerer)
	{
		if (triggerer is Tool tool)
		{
			// TransformFood(tool.GetToolType());
		}
	}

	private void TransformFood(EToolType toolType)
	{
		foreach (var transform in foodStates)
		{
			if (transform.toolType == toolType &&
					transform.requiredState == currentIngredient.state)
			{
				currentIngredient.SetState(transform.resultingState);
				Debug.Log($"Transformed to: {currentIngredient}");
				return;
			}
		}
		Debug.Log($"No valid transformation for {toolType} on {currentIngredient}");
	}

	[System.Serializable]
	private class FoodStateTransform
	{
		public EToolType toolType;
		public EIngredientState requiredState;
		public EIngredientState resultingState;
	}
}