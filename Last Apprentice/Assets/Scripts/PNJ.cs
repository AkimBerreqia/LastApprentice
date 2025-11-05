using UnityEngine;

public class PNJ : Entity, InteractionManager
{
    public PNJ(string Name, string[] Messages) : base(Name, Messages)
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

    public void InteractWith()
    {
        Speak();
    } 

    private void Speak()
    {
        Debug.Log("hey! I'm speaking... \n- " + Name);
    }
}
