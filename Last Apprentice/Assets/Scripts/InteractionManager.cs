using Unity.VisualScripting;
using UnityEngine;

public interface InteractionManager
{
    public void InteractWith(GameObject interactable)
    {
        if (interactable.CompareTag("PNJ"))
        {
            interactable.GetComponent<PNJ>().InteractWith();
        }
    }
}
