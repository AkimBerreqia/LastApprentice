using UnityEngine;

public class NPC : Entity, InteractionManager
{
    public NPC(string Name, string[] Messages) : base(Name, Messages)
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

    public void InteractWith(Collider2D interactableCollider)
    {
        if (interactableCollider.CompareTag("Player"))
        {
            Speak();
        }
    } 

    public void Speak()
    {
        Debug.Log("hey! I'm speaking... \n- " + Name);
    }
}
