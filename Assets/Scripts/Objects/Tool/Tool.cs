using UnityEngine;

public abstract class Tool : MonoBehaviour, IInteractionTriggerer
{
  public abstract Food[] InteractableFoods {  get; protected set; }
  public abstract void IInteractionTrigger(IInteractionListener listener);
}
