using UnityEngine;

public class Player : MonoBehaviour
{
    public int Health = 100;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddHealth(int health)
    {
        Health += health;
        Debug.Log("Your Health:" + health);
    }

}
