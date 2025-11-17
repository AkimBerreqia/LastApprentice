using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour, InteractionManager
{
    [SerializeField] private InteractionManager interactionManager;
    [SerializeField] private Collider2D playerCollider;

    public PlayerInteraction()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        InteractWith(collision);
    }

    public void InteractWith(Collider2D interactableCollider)
    {
        if (interactableCollider.gameObject.CompareTag("PNJ"))
        {
            interactableCollider.GetComponent<NPC>().InteractWith(playerCollider);
        }
    }
}
