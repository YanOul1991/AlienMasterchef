using UnityEngine;

public enum EToolType
{
  Knife,
  Blender
}

public class Food : MonoBehaviour, IInteractionListener
{
  [SerializeField] private FoodStateTransform[] FoodStates;

  public void InteractionListen(IInteractionTriggerer triggerer)
  {

  }

  [System.Serializable]
  private class FoodStateTransform
  {
    [SerializeField] public EToolType toolType;
    [SerializeField] public EBaseIngredient resultingIngredient;
  }
}