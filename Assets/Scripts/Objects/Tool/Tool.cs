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
}