using UnityEngine;

public abstract class Food : MonoBehaviour, IInteractionListener
{
  public abstract EBaseIngredient BaseIngredientType { get; }
  protected abstract void IngredientTransform();
  protected abstract void IngredientCook();

  // Interface
  public abstract void InteractionListen(IInteractionTriggerer triggerer);
}