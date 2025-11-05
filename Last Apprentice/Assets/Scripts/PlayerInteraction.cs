using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour, InteractionManager
{
    private InteractionManager interactionManager;
    private GameObject interactable;

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
        interactable = collision.gameObject;
        interactionManager.InteractWith(interactable);
    }
}
