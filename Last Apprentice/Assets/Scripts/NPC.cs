using UnityEngine;

public class NPC : Entity, IInteractable
{
    [SerializeField] private PlayerInteraction player;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ShowPopUp();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<PlayerInteraction>();
            player.SetInteractable(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<PlayerInteraction>();
            player.ClearInteractable();
            HidePopUp();
        }
    }
    
    public void InteractWith()
    {

    }

    public void InteractWithWhenEntering()
    {
        ShowPopUp();
    }

    public void InteractWithWhenStaying()
    {
        //if input down for interaction, then speak !
    }

    public void InteractWithWhenLeaving()
    {
        HidePopUp();
    }

    private void ShowPopUp()
    {

    }

    private void HidePopUp()
    {

    }
    
    public void Speak()
    {
        Debug.Log("hey! I'm speaking... \n- " + Name);
    }
}
