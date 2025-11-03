using UnityEngine;

public class Entity : Interactable
{
    public string Name;
    private string[] Messages;
    protected Entity(string Name, string[] Messages)
    {
        this.Name = Name;
        this.Messages = Messages;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void InteractWith()
    {
        
    }
}
