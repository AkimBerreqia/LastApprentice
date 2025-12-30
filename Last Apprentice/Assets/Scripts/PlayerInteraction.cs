using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private IInteractable interactable;

    public void TryToInteractWith()
    {
        if (interactable != null)
        {
            interactable.InteractWith();
        }
    }

    public void SetInteractable(IInteractable instance)
    {
        interactable = instance;
    }

    public void ClearInteractable()
    {
        interactable = null;
    }
}
