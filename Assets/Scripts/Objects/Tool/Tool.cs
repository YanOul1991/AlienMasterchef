using UnityEngine;

public class Tool : MonoBehaviour, IInteractionTriggerer
{
    [SerializeField] private EToolType toolType;

    public EToolType GetToolType()
    {
        return toolType;
    }

    public void IInteractionTrigger(IInteractionListener listener)
    {
        listener.InteractionListen(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Food food = other.GetComponent<Food>();
        if (food != null)
        {
            IInteractionTrigger(food);
        }
    }
}