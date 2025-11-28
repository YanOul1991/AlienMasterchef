using UnityEngine;

public abstract class Tool : MonoBehaviour, IInteractionTriggerer
{
  public abstract Food[] InteractableFoods {  get; protected set; }

  // Interface
  public abstract void IInteractionTrigger(IInteractionListener listener);
}
