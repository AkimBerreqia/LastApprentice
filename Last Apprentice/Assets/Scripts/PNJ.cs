using UnityEngine;

public class PNJ : Entity
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

    protected new void InteractWith()
    {
        Speak();
    }

    private void Speak()
    {
        Debug.Log("hey! I'm speaking... \n- " + Name);
    }
}
